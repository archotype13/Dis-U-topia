public sealed class MoveAction(Point newCords, int speed) : EntityAction // uses the move system to move the entity
{
    private readonly Point _newCords = newCords;
    private readonly int _speed = speed;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position != null)
        {
            if (actor.Position.Move(_newCords))
                return new SucceededActionResult( GetActionCost(Engine.Instance!.GameManager.CurrentLevel!.Grid.GetCell(_newCords).MoveCost, _speed) );
                // move energy is calculated by dividing by move cost of the tile by the speed divided by the average speed, speed limit is 5 energy points for 20 tiles per turn

        }
        return new FailedActionResult();
    }
}

public sealed class WaitAction() : EntityAction // uses up all of the entities remaining energy to pass the turn
{
    public override ActionResult Perform(Entity actor)
    {
        return new SucceededActionResult(actor.Ai!.Energy);
    }
}

public sealed class PathMoveAction(Point targetCords, int speed, int maxVisits = -1) : EntityAction // makes the entity pathfind and move towards a target point
{
    private readonly Point _targetCords = targetCords;
    private readonly int _speed = speed;
    private readonly int _maxVisits = maxVisits;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position == null)
            return new FailedActionResult();

        List<Point> path = AStar.GetPathTo(actor.Position!.Cords, _targetCords, Engine.Instance!.GameManager.CurrentLevel!.Grid, true, _maxVisits);
        Engine.Instance.ScreenManager.WorldView.debugDrawPoints = path;
        if (path.Count > 0)
            return new AlternativeActionResult( new MoveAction(path.First(), _speed) );
        return new FailedActionResult();
    }
}

public sealed class ToggleDoorAction(Entity door, int quickness, bool open) : EntityAction // toggle a door's state
{
    private Entity _door = door;
    private readonly int _quickness = quickness;
    private readonly bool _open = open;

    public override ActionResult Perform(Entity actor)
    {
        if (_door.Door!.ToggleState(_door, _open))
            return new SucceededActionResult(GetActionCost(_door.Door.OpenCost, _quickness));
        return new FailedActionResult();
    }
}

public sealed class MoveOrAttackAction(Point targetCords, int speed, int quickness, bool openDoors) : EntityAction // default walking action for most AIs. Will either open a door, attack an enemy, or move depending on context
{
    private readonly Point _targetCords = targetCords;
    private readonly int _speed = speed;
    private readonly int _quickness = quickness;
    private readonly bool _openDoors = openDoors;

    public override ActionResult Perform(Entity actor)
    {
        List<Entity> entities = Engine.Instance!.GameManager.CurrentLevel!.GetEntitiesAt(_targetCords);

        foreach (Entity entity in entities)
        {
            if (_openDoors == true && entity.Door != null && !entity.Door.IsOpened) // open a closed door if possible
                return new AlternativeActionResult(new ToggleDoorAction(entity, _quickness, true));
        }

        // if no doors or attackable entities can be found, move
        return new AlternativeActionResult( new MoveAction(_targetCords, _speed) );
    }
}

public sealed class PathOrAttackAction(Point targetCords, int maxVisits, int speed, int quickness, bool openDoors) : EntityAction
{
    private readonly Point _targetCords = targetCords;
    private readonly int _maxVisits = maxVisits;
    private readonly int _speed = speed;
    private readonly int _quickness = quickness;
    private readonly bool _openDoors = openDoors;

    public override ActionResult Perform(Entity actor)
    {
        List<Point> path = AStar.GetPathTo(actor.Position!.Cords, _targetCords, Engine.Instance!.GameManager.CurrentLevel!.Grid, true, _maxVisits);
        Engine.Instance.ScreenManager.WorldView.debugDrawPoints = path;
        if (path.Count > 0)
            return new AlternativeActionResult( new MoveOrAttackAction(path.First(), _speed, _quickness, _openDoors) );
        return new FailedActionResult();
    }
}
