using System.Diagnostics;

public sealed class GameManager : ScreenObject // manages game state, turn order of entities, and the current level
{
    public Entity? Player;
    public Level? CurrentLevel;

    public GameState CurrentState = GameState.PLAYER_TURN;
    public enum GameState
    {
        PLAYER_TURN,
        TARGETING,
        NEW_TURN,
        SELECTION
    };
    public Selector? Selector;

    public const int MAX_MESSAGES = 50;
    public List<string> LoggedMessages {get; private set;} = [];

    private ulong _ticks = 0;

    public override void Update(TimeSpan delta)
    {
        if (CurrentLevel != null)
        {
            // redraw level
            Engine.Instance!.ScreenManager.WorldView.RedrawLevel(CurrentLevel);
            Tick();
        }
        base.Update(delta);
    }

    // do turn order
    private void Tick()
    {
        // perform the player's turn to allow them to take their time
        if (CurrentState == GameState.PLAYER_TURN)
        {
            TickPlayerDecision();
            CurrentLevel?.FlushDeletedEntities();
        }

        // tick through all the non-player entities to perform their turns
        if (CurrentState == GameState.NEW_TURN)
        {
            TickNewRound();
            CurrentLevel?.FlushDeletedEntities();
        }

        // handle tile targeting!
        if (CurrentState == GameState.TARGETING)
            TickSelection();

    }

    private void TickPlayerDecision() // make player take actions and make decisions
    {
        if (Player != null)
        {
            if (Player.Ai!.Energy <= 0)
                CurrentState = GameState.NEW_TURN;

            TickEntity(Player, false);
        }
    }

    private void TickNewRound() // let all entities take their turns and be ticked
    {
        long startTime = Stopwatch.GetTimestamp(); // get time for perfomance debugging

        foreach (Entity entity in CurrentLevel!.Entities)
        {
            if (entity != Player && entity.Ai != null)
            {
                entity.Ai!.Energy += 100; // add their energy
                while (entity.Ai.Energy > 0)
                    TickEntity(entity);
            }
        }
        _ticks++;

        System.Console.WriteLine($"Round {_ticks}\nElasped turn time: {Stopwatch.GetElapsedTime(startTime)}"); // print out time it took for round to process for turn time

        Player!.Ai!.Energy += 100; // add player energy
        CurrentState = GameState.PLAYER_TURN;
    }

    private void TickEntity(Entity entity, bool endTurnOnFail = true) // performs a single entity's turn
    {
        EntityAction action = entity.Ai!.Turn(entity);
        EntityPerformAction(entity, action, endTurnOnFail);
    }

    public ActionResult EntityPerformAction(Entity entity, EntityAction action, bool endTurnOnFail = true) // performs an action for an entity and handles the results
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
        return result;
    }

    private void TickSelection() // ticks for player selection
    {
        if (Selector != null)
        {
            if (Selector.Select() == true)
            {
                CurrentState = GameState.PLAYER_TURN;
            }
        }
    }

    public void Save(BinaryWriter writer)
    {
        // save ticks
        writer.Write(_ticks);
        // save player
        Player!.Save(writer);
        // save level
        writer.Write(CurrentLevel != null);
        if (CurrentLevel != null)
        {
            writer.Write(CurrentLevel.Width);
            writer.Write(CurrentLevel.Height);
            CurrentLevel?.Save(writer);
        }
        // save messages
        writer.Write(LoggedMessages.Count);
        foreach (string message in LoggedMessages)
        {
            writer.Write(message);
        } 
    }

    public void Load(BinaryReader reader)
    {
        // load the ticks
        _ticks = reader.ReadUInt64();
        // load the player
        Player = new();
        Player.Load(reader);
        if (reader.ReadBoolean() == true) // load level if there is one
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            CurrentLevel = new(width, height);
            CurrentLevel.Load(reader);

            CurrentLevel.AddEntity(Player); // add the player :3
        }
        // load messages
        int messageCount = reader.ReadInt32();
        for (int i = 0; i < messageCount; i++)
        {
            LoggedMessages.Add(reader.ReadString());
            Engine.Instance!.ScreenManager.Log.LogMessage(LoggedMessages[i], false);
        }
    }
}