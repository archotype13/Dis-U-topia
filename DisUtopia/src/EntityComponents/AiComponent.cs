using Newtonsoft.Json.Serialization;
using SadConsole.Input;

public class AiComponent : EntityComponent
{
    public int Speed = 100;
    public int Quickness = 100;
    public int Energy = 0;
    protected enum AiType
    {
        BLANK,
        PLAYER,
        BASIC
    };

    public virtual EntityAction Turn(Entity owner)
    {
        Engine.Instance!.ScreenManager.Log.LogMessage($"It's {owner.Name} turn!");
        return new WaitAction();
    }

    public static AiComponent Create(BinaryReader reader)
    {
        AiType type = (AiType)reader.ReadInt32();
        AiComponent ai;
        switch (type)
        {
            case AiType.PLAYER:
                ai = new PlayerAiComponent();
                break;
            case AiType.BASIC:
                ai = new BasicAiComponent();
                break;
            default:
                ai = new AiComponent();
                break;
        }
        ai.Load(reader);
        return ai;
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Speed);
        writer.Write(Quickness);
        writer.Write(Energy);
    }

    public override void Load(BinaryReader reader)
    {
        Speed = reader.ReadInt32();
        Quickness = reader.ReadInt32();
        Energy = reader.ReadInt32();
    }
}

public class PlayerAiComponent : AiComponent
{
    public override EntityAction Turn(Entity owner)
    {
        // movement
        Point dPos = (0,0);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad8) )
            dPos = (0, -1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad9) )
            dPos = (1, -1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad6) )
            dPos = (1, 0);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad3) )
            dPos = (1, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad2) )
            dPos = (0, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad1) )
            dPos = (-1, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad4) )
            dPos = (-1, 0);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad7) )
            dPos = (-1, -1);

        // actually move if a direction was decided upon
        if ( dPos != (0, 0) )
        {
            return new MoveOrAttackAction(dPos + owner.Position!.Cords, true, Engine.Keyboard.IsKeyDown(Keys.LeftControl), true);
        }
        // waiting
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad5) )
        {
            return new WaitAction();
        }
        // inventory
        if ( Engine.Keyboard.IsKeyPressed(Keys.I) )
        {
            InventoryWindow.Create();
        }
        // toggling doors
        if ( Engine.Keyboard.IsKeyPressed(Keys.C) ) // closing
        {
            ToggleDoorSelection(owner, false);
            return new EntityAction();
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.O) ) // opening
        {
            ToggleDoorSelection(owner, true);
            return new EntityAction();
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.G) ) // grab items
        {
            return GrabItems(owner);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.D) ) // drop items
        {
            return DropItems(owner);
        }

        // debug cheats
        if ( Engine.Keyboard.IsKeyPressed(Keys.F1))
            Engine.Instance!.GameManager.IgnoreLOS = !Engine.Instance!.GameManager.IgnoreLOS;
        
        return new EntityAction();
    }

    public void ToggleDoorSelection(Entity owner, bool open)
    {
        Action<Point> action = (point) =>
        {
            point += owner.Position!.Cords;
            foreach (Entity entity in Engine.Instance!.GameManager.CurrentLevel!.GetEntitiesAt(point) )
            {
                if ( entity.Door != null && entity.Door.IsOpened != open)
                {
                    ActionResult result = Engine.Instance!.GameManager.EntityPerformAction(owner, new ToggleDoorAction(entity, open), open);
                    // opening and closing messages
                    if (result is SucceededActionResult)
                        Engine.Instance!.ScreenManager.Log.LogMessage($"You {(open? "open" : "close")} the {entity.Name}");
                    else
                        Engine.Instance!.ScreenManager.Log.LogMessage($"The door doesn't budge");

                    return;
                }
            }
            Engine.Instance!.ScreenManager.Log.LogMessage($"There isn't anything to {(open? "open" : "close")} there");
        };
        Engine.Instance!.GameManager.Selector = new DirectionSelector(action);
        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.TARGETING;
    }

    public EntityAction GrabItems(Entity owner)
    {
        // get items
        List<Entity> items = [.. Engine.Instance!.GameManager.CurrentLevel!.GetEntitiesAt(owner.Position!.Cords).Where(e => e.Item != null)];
        if (items.Count == 1) // if there's just one item, just pick it up!
        {
            return new PickUpItemAction(items[0]);
        }
        if (items.Count > 0)  // if there's more than one item, create a selection window
        {
            int height = Math.Min(items.Count + 2, Game.Instance.ScreenCellsY);
            EntitySelectionWindow selectionWindow = EntitySelectionWindow.Create(Game.Instance.ScreenCellsX / 2 - GeneralConstants.PICKUP_DROP_WINDOW_WIDTH / 2, Game.Instance.ScreenCellsY / 2 - height / 2, GeneralConstants.PICKUP_DROP_WINDOW_WIDTH, height, items, "Pick up what?", true);
            selectionWindow.List.SelectedItemExecuted += (s, a) => {Engine.Instance!.GameManager.EntityPerformAction(owner, new PickUpItemAction((Entity)a.Item!)); selectionWindow.Close();};
        }
        else // else, just print a message
        {
            Engine.Instance!.ScreenManager.Log.LogMessage("There's nothing here to pick up!");
        }
        
        return new EntityAction();
    }

    public EntityAction DropItems(Entity owner)
    {
        EntitySelectionWindow selectionWindow = EntitySelectionWindow.Create(Game.Instance.ScreenCellsX / 2 - GeneralConstants.PICKUP_DROP_WINDOW_WIDTH / 2, 0, GeneralConstants.PICKUP_DROP_WINDOW_WIDTH, Game.Instance.ScreenCellsY, owner.Inventory!.Items, "Drop what?", true);
        selectionWindow.List.SelectedItemExecuted += (s, a) => {Engine.Instance!.GameManager.EntityPerformAction(owner, new DropItemAction((Entity)a.Item!)); selectionWindow.Close();};
        return new EntityAction();
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write((int)AiType.PLAYER);
        base.Save(writer);
    }
}

public class BasicAiComponent : AiComponent
{
    public override EntityAction Turn(Entity owner)
    {
        return new PathOrAttackAction(Engine.Instance!.GameManager.Player!.Position!.Cords, 100, true);
        // return new PathMoveAction(Engine.Instance!.GameManager.Player!.Position!.Cords, owner.Ai!.Speed, 100);
        // return new MoveAction(owner.Position!.Cords + (Engine.Rng.Next(0, 3) - 1, Engine.Rng.Next(0, 3) - 1), Speed);
        // Engine.Instance!.GameManager.CurrentLevel!.RemoveEntity(owner);
        // return new WaitAction();
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write((int)AiType.BASIC);
        base.Save(writer);
    }
}
