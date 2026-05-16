using SadConsole.Input;

public abstract class Ai(Actor owner)
{
    public int Energy = 0; // amount of points the actor has to spend on it's turn
    public int Speed = 100; // amount of energy the actor gains on it's turn
    protected Actor _owner = owner;
    public abstract Action TakeTurn(); // decide what action to take on its turn
    public void GainEnergy() // Gain energy at the start of it's turn (NOTE: USE THIS FOR ANY EFFECTS THAT OCCUR AT THE START OF THE TURN)
    {
        Energy += 100;
    }
}

public class PlayerAi : Ai
{
    public override Action TakeTurn()
    {
        // movement
        Point deltaPos = (0, 0);
        if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad8) || Engine.Keyboard!.IsKeyPressed(Keys.Up) || Engine.Keyboard!.IsKeyPressed(Keys.K))
        {
            deltaPos = (0, -1);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad9) || Engine.Keyboard!.IsKeyPressed(Keys.U))
        {
            deltaPos = (1, -1);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad6) || Engine.Keyboard!.IsKeyPressed(Keys.Right) || Engine.Keyboard!.IsKeyPressed(Keys.L))
        {
            deltaPos = (1, 0);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad3) || Engine.Keyboard!.IsKeyPressed(Keys.N))
        {
            deltaPos = (1, 1);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad2) || Engine.Keyboard!.IsKeyPressed(Keys.Down) || Engine.Keyboard!.IsKeyPressed(Keys.J))
        {
            deltaPos = (0, 1);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad1) || Engine.Keyboard!.IsKeyPressed(Keys.B))
        {
            deltaPos = (-1, 1);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad4) || Engine.Keyboard!.IsKeyPressed(Keys.Left) || Engine.Keyboard!.IsKeyPressed(Keys.H))
        {
            deltaPos = (-1, 0);
        }
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad7) || Engine.Keyboard!.IsKeyPressed(Keys.Y))
        {
            deltaPos = (-1, -1);
        
        }
        // other actions
        else if (Engine.Keyboard!.IsKeyPressed(Keys.NumPad5) || Engine.Keyboard!.IsKeyPressed(Keys.OemPeriod)) // waiting
        {
            return new WaitAction(_owner);
        }

        if (deltaPos != (0,0))
        {
            return new MoveAction(_owner, _owner.Position + deltaPos);
        }
        
        // return a blank action to keep passing your turn
        return new Action(_owner);
    }

    public PlayerAi(Actor owner) : base(owner)
    {
        
    }
}

public class MobAi : Ai
{
    public override Action TakeTurn()
    {
        List<Point> path = AStar.GetPathTo(_owner.Position, Engine.Player!.Position, Engine.Map!, Engine.Map!.IsSolidAt, true, 25);
        Engine.DebugHighlights = path;
        if (path.Count > 0)
            return new MoveAction(_owner, path.First());
        return new WaitAction(_owner);
    }

    public MobAi(Actor owner) : base(owner) {}
}

