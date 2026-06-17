using System.Diagnostics;

public class GameManager : ScreenObject // manages game state, turn order of entities, and the current level
{
    public Entity? Player;
    public Level? CurrentLevel;

    public GameState CurrentState = GameState.PLAYER_TURN;
    public enum GameState
    {
        PLAYER_TURN,
        NEW_TURN
    };

    public override void Update(TimeSpan delta)
    {
        if (CurrentLevel != null)
        {
            // redraw level
            Engine.Instance!.ScreenManager.WorldView.RedrawLevel(CurrentLevel);
            TickRound();
        }
        base.Update(delta);
    }

    // do turn order
    private void TickRound()
    {
        if (CurrentState == GameState.PLAYER_TURN) // perform the player's turn to allow them to take their time
        {
            if (Player != null)
            {
                TickEntity(Player, false);

                if (Player.Ai!.Energy <= 0)
                    CurrentState = GameState.NEW_TURN;
            }
        }
        if (CurrentState == GameState.NEW_TURN) // tick through all the non-player entities to perform their turns
        {
            long startTime = Stopwatch.GetTimestamp(); // get time for perfomance debugging

            foreach (Entity entity in CurrentLevel!.Entities)
            {
                if (entity != Player && entity.Ai != null)
                {
                    entity.Ai.Energy += 100; // add their energy
                    while (entity.Ai.Energy > 0)
                        TickEntity(entity);
                }
            }

            System.Console.WriteLine($"Elasped turn time: {Stopwatch.GetElapsedTime(startTime)}"); // print out time it took for round to process for turn time

            Player!.Ai!.Energy += 100; // add player energy
            CurrentState = GameState.PLAYER_TURN;
        }

    }

    private void TickEntity(Entity entity, bool endTurnOnFail = true) // performs a single entity's turn
    {
        EntityAction action = entity.Ai!.Turn(entity);
        EntityPerformAction(entity, action, endTurnOnFail);
        // ActionResult result = action.Perform(entity);
        
        // // handle results
        // switch (result)
        // {
        //     case SucceededActionResult succeeded: // if succeeded, remove energy cost
        //         entity.Ai.Energy -= succeeded.UsedEnergy;
        //         break;
        //     case FailedActionResult failed: // if failed return
        //         if (endTurnOnFail)
        //             entity.Ai.Energy = 0;
        //         break;
        //     case AlternativeActionResult alternativeAction:
                
        //         break;
        //     default: break;
        // }
    }

    private void EntityPerformAction(Entity entity, EntityAction action, bool endTurnOnFail = true) // performs an action for an entity and handles the results
    {
        ActionResult result = action.Perform(entity);
        
        // handle results
        switch (result)
        {
            case SucceededActionResult succeeded: // if succeeded, remove energy cost
                entity.Ai!.Energy -= succeeded.UsedEnergy;
                break;
            case FailedActionResult: // if failed return
                if (endTurnOnFail)
                    entity.Ai!.Energy = 0;
                break;
            case AlternativeActionResult alternativeAction: // if alternate action, perform that new action
                EntityPerformAction(entity, alternativeAction.NewAction, endTurnOnFail);
                break;
            default: break;
        }
    }
}