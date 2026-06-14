public class EntityAction
{
    public virtual ActionResult Perform(Entity actor)
    {
        return new SucceededActionResult(0);
    }
}

public class ActionResult
{
    
}

public class SucceededActionResult(int usedEnergy): ActionResult
{
    public int UsedEnergy = usedEnergy;
}

public class FailedActionResult() : ActionResult
{
}