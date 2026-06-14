public class MoveAction(Point newCords, int speed) : EntityAction // uses the move system to move the entity
{
    private Point _newCords = newCords;
    private int _speed = speed;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position != null)
            actor.Position.Cords = _newCords;
        return new SucceededActionResult(_speed);
    }
}

public class WaitAction() : EntityAction // uses up all of the entities remaining energy to pass the turn
{
    public override ActionResult Perform(Entity actor)
    {
        return new SucceededActionResult(actor.Ai!.Energy);
    }
}