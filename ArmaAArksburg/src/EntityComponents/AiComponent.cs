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
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad8) )
        {
            return new MoveAction(owner.Position!.Cords + (0, -1), Speed);
        }
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad2) )
        {
            return new MoveAction(owner.Position!.Cords + (0, 1), Speed);
        }
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
