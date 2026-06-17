public class MoveAction(Point newCords, int speed) : EntityAction // uses the move system to move the entity
{
    private Point _newCords = newCords;
    private int _speed = speed;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position != null)
        {
            if (actor.Position.Move(_newCords))
                return new SucceededActionResult( Math.Max(Engine.Instance!.GameManager.CurrentLevel!.Grid.GetCell(_newCords).Item2 / (actor.Ai!.Speed / 100), 5 ) );
                // move energy is calculated by dividing by move cost of the tile by the speed divided by the average speed, speed limit is 5 energy points for 20 tiles per turn

        }
        return new FailedActionResult();
    }
}

public class WaitAction() : EntityAction // uses up all of the entities remaining energy to pass the turn
{
    public override ActionResult Perform(Entity actor)
    {
        return new SucceededActionResult(actor.Ai!.Energy);
    }
}