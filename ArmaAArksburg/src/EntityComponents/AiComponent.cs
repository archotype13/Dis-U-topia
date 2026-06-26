using SadConsole.Input;

public class AiComponent : EntityComponent
{
    public int Speed = 100;
    public int Quickness = 100;
    public int Energy = 0;
    public virtual EntityAction Turn(Entity owner)
    {
        Engine.Instance!.ScreenManager.Log.LogMessage($"It's {owner.Name} turn!");
        return new WaitAction();
    }

    public override void AddToLevel(Entity owner, Level level)
    {
        level.AIs.Add(owner);
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
            return new MoveOrAttackAction(dPos + owner.Position!.Cords, Speed, Quickness, true);
        }
        // waiting
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad5))
        {
            return new WaitAction();
        }
        
        return new EntityAction();
    }
}

public class DrunkAiComponent : AiComponent
{
    public override EntityAction Turn(Entity owner)
    {
        return new PathOrAttackAction(Engine.Instance!.GameManager.Player!.Position!.Cords, 100, owner.Ai!.Speed, owner.Ai.Quickness, true);
        // return new PathMoveAction(Engine.Instance!.GameManager.Player!.Position!.Cords, owner.Ai!.Speed, 100);
        // return new MoveAction(owner.Position!.Cords + (Engine.Rng.Next(0, 3) - 1, Engine.Rng.Next(0, 3) - 1), Speed);
    }
}
