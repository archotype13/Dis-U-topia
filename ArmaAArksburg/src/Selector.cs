public class Selector
{
    public EntityAction? action;
    public virtual void Select(GameManager gameManager) // change engine state to cancel selection
    {
        gameManager.CurrentState = GameManager.GameState.PLAYER_TURN;
    }
}

public class DirectionSelector : Selector // returns a direction point + the player's position
{
    private Point referencePoint;
    public override void Select(GameManager gameManager) // change engine state to cancel selection
    {
        return gameManager.Player!.Position!.Cords + (1, 0);
    }

    public DirectionSelector()
    {
        
    }
}