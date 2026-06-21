public class EntityAction
{
    public virtual ActionResult Perform(Entity actor)
    {
        return new SucceededActionResult(0);
    }

    protected int GetActionCost(int baseCost, int speed)
    {
        return Math.Max(baseCost * 100 / speed, 5 );
    }
}

public class ActionResult
{
    
}

public class SucceededActionResult(int usedEnergy): ActionResult // returns used energy
{
    public int UsedEnergy = usedEnergy;
}

public class FailedActionResult() : ActionResult // does nothing for player but immediately ends an entity's turn
{
}

public class AlternativeActionResult(EntityAction newAction) : ActionResult // makes the entity take a different action
{
    public EntityAction NewAction = newAction;
}