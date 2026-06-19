public sealed class MoveAction(Point newCords, int speed) : EntityAction // uses the move system to move the entity
{
    private Point _newCords = newCords;
    private int _speed = speed;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position != null)
        {
            if (actor.Position.Move(_newCords))
                return new SucceededActionResult( Math.Max(Engine.Instance!.GameManager.CurrentLevel!.Grid.GetCell(_newCords).MoveCost / (_speed / 100), 5 ) );
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
    private Point _targetCords = targetCords;
    private int _speed = speed;
    private int _maxVisits = maxVisits;
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
