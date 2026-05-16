using System.Reflection.Metadata;

public class Action
{
    protected Actor _owner;
    public virtual int Perform()
    {
        return 0;
    }

    public Action(Actor owner)
    {
        _owner = owner;
    }
}

public class MoveAction : Action
{
    protected Point _newPosition;
    public override int Perform()
    {
        if (_owner.Move(_newPosition) || _owner != Engine.Player) // return the speed if the move succeeds or owner isn't player to avoid infinite loops
            return _owner.Ai!.Speed;
        return 0;
    }

    public MoveAction(Actor owner, Point newPosition) : base(owner)
    {
        _newPosition = newPosition;
    }
}

public class WaitAction : Action
{
    public override int Perform()
    {
        return _owner.Ai!.Energy;
    }

    public WaitAction(Actor owner) : base(owner)
    {}
}
