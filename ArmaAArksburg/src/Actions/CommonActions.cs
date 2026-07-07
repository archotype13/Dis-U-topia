public sealed class MoveAction(Point newCords) : EntityAction // uses the move system to move the entity
{
    private readonly Point _newCords = newCords;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position != null)
        {
            if (actor.Position.Move(_newCords))
                return new SucceededActionResult( GetActionCost(Engine.Instance!.GameManager.CurrentLevel!.Grid!.GetCell(_newCords).MoveCost, actor.Ai!.Speed) );
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

public sealed class PathMoveAction(Point targetCords, int maxVisits = -1) : EntityAction // makes the entity pathfind and move towards a target point
{
    private readonly Point _targetCords = targetCords;
    private readonly int _maxVisits = maxVisits;
    public override ActionResult Perform(Entity actor)
    {
        if (actor.Position == null)
            return new FailedActionResult();

        List<Point> path = AStar.GetPathTo(actor.Position!.Cords, _targetCords, Engine.Instance!.GameManager.CurrentLevel!.Grid!, true, _maxVisits);
        Engine.Instance.ScreenManager.WorldView.debugDrawPoints = path;
        if (path.Count > 0)
            return new AlternativeActionResult( new MoveAction(path.First()) );
        return new FailedActionResult();
    }
}

public sealed class ToggleDoorAction(Entity door, bool open) : EntityAction // toggle a door's state
{
    private Entity _door = door;
    private readonly bool _open = open;

    public override ActionResult Perform(Entity actor)
    {
        if (_door.Door!.ToggleState(_door, _open))
            return new SucceededActionResult(GetActionCost(_door.Door.OpenCost, actor.Ai!.Quickness));
        return new FailedActionResult();
    }
}

public sealed class MoveOrAttackAction(Point targetCords, bool openDoors, bool forced = false) : EntityAction // default walking action for most AIs. Will either open a door, attack an enemy, or move depending on context
{
    private readonly Point _targetCords = targetCords;
    private readonly bool _openDoors = openDoors;

    public override ActionResult Perform(Entity actor)
    {
        List<Entity> entities = Engine.Instance!.GameManager.CurrentLevel!.GetEntitiesAt(_targetCords);

        foreach (Entity entity in entities)
        {
            if (actor.Attack != null && HealthManager.IsTargetable(entity, forced)) // check for if the entity is a valid target to melee attack and has an actual melee attack
            {
                return new AlternativeActionResult( new AttackAction(entity, 100, actor.Attack.Attack) );
            }
            else if (_openDoors == true && entity.Door != null && !entity.Door.IsOpened) // open a closed door if possible
                return new AlternativeActionResult(new ToggleDoorAction(entity, true));
        }

        // check for destructible walls
        if (actor.Attack != null && HealthManager.IsTileTargetable(Engine.Instance!.GameManager.CurrentLevel!, _targetCords, forced))
            return new AlternativeActionResult( new AttackTileAction(_targetCords, 100, actor.Attack.Attack) );

        // if no doors or attackable entities can be found, move
        return new AlternativeActionResult( new MoveAction(_targetCords) );
    }
}

public sealed class PathOrAttackAction(Point targetCords, int maxVisits, bool openDoors) : EntityAction
{
    private readonly Point _targetCords = targetCords;
    private readonly int _maxVisits = maxVisits;
    private readonly bool _openDoors = openDoors;

    public override ActionResult Perform(Entity actor)
    {
        List<Point> path = AStar.GetPathTo(actor.Position!.Cords, _targetCords, Engine.Instance!.GameManager.CurrentLevel!.Grid!, true, _maxVisits);
        Engine.Instance.ScreenManager.WorldView.debugDrawPoints = path;
        if (path.Count > 0)
            return new AlternativeActionResult( new MoveOrAttackAction(path.First(), _openDoors) );
        return new FailedActionResult();
    }
}

public sealed class AttackAction(Entity target, int baseCost, AttackData attack) : EntityAction
{
    private readonly Entity Target = target;
    private readonly int BaseCost = baseCost;
    private readonly AttackData Attack = attack;

    public override ActionResult Perform(Entity actor)
    {
        HealthManager.DealDamage(Target, actor, Attack);
        return new SucceededActionResult(GetActionCost(BaseCost, actor.Ai!.Quickness));
    }
}

public sealed class AttackTileAction(Point target, int baseCost, AttackData attack) : EntityAction // make sure the target tile is actually valid!!!
{
    private readonly Point Target = target;
    private readonly int BaseCost = baseCost;
    private readonly AttackData Attack = attack;

    public override ActionResult Perform(Entity actor)
    {
        HealthManager.DealDamageToTile(Engine.Instance!.GameManager.CurrentLevel!, Target, Attack, actor);
        return new SucceededActionResult(GetActionCost(BaseCost, actor.Ai!.Quickness));
    }
}
