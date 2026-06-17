using SadConsole.Input;

public class AiComponent
{
    public int Speed = 100;
    public int Energy;
    public virtual EntityAction Turn(Entity owner)
    {
        Engine.Instance!.ScreenManager.Log.LogMessage($"It's {owner.Name} turn!");
        return new WaitAction();
    }
}

public class PlayerAiComponent : AiComponent
{
    public override EntityAction Turn(Entity owner)
    {
        // movement
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad8) )
        {
            return new MoveAction(owner.Position!.Cords + (0, -1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad9) )
        {
            return new MoveAction(owner.Position!.Cords + (1, -1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad6) )
        {
            return new MoveAction(owner.Position!.Cords + (1, 0), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad3) )
        {
            return new MoveAction(owner.Position!.Cords + (1, 1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad2) )
        {
            return new MoveAction(owner.Position!.Cords + (0, 1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad1) )
        {
            return new MoveAction(owner.Position!.Cords + (-1, 1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad4) )
        {
            return new MoveAction(owner.Position!.Cords + (-1, 0), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad7) )
        {
            return new MoveAction(owner.Position!.Cords + (-1, -1), Speed);
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
        return new MoveAction(owner.Position!.Cords + (Engine.Rng.Next(0, 3) - 1, Engine.Rng.Next(0, 3) - 1), Speed);
    }
}
