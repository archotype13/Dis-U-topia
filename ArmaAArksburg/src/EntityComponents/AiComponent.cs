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
