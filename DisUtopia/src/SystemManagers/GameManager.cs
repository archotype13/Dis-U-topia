using System.Diagnostics;

public sealed class GameManager : ScreenObject // manages game state, turn order of entities, and the current level
{
    public Entity? Player;
    public Level? CurrentLevel;

    public GameState CurrentState = GameState.START_UP;
    public enum GameState
    {
        START_UP,
        PLAYER_TURN,
        TARGETING,
        NEW_TURN,
        UI,
        GAME_OVER
    };
    public Selector? Selector;

    public const int MAX_MESSAGES = 50;
    public List<string> LoggedMessages {get; private set;} = [];

    private ulong _ticks = 0;

    // debug cheats
    public bool IgnoreLOS = false;

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
        // start up
        if (CurrentState == GameState.START_UP)
        {
            OnPlayerAction();
            CurrentState = GameState.PLAYER_TURN;
        }

        // perform the player's turn to allow them to take their time
        if (CurrentState == GameState.PLAYER_TURN)
        {
            TickPlayerDecision();
            CurrentLevel?.FlushEntities();
        }

        // tick through all the non-player entities to perform their turns
        if (CurrentState == GameState.NEW_TURN)
        {
            TickNewRound();
            CurrentLevel?.FlushEntities();
        }

        // handle tile targeting!
        if (CurrentState == GameState.TARGETING)
            TickSelection();
    }

    private void TickPlayerDecision() // make player take actions and make decisions
    {
        if (Player != null)
        {
            if (!HealthManager.IsAlive(Player)) // manage player death
            {
                CurrentState = GameState.GAME_OVER;
                return;
            }


            if (Player.Ai!.Energy <= 0)
                CurrentState = GameState.NEW_TURN;

            if (TickEntity(Player, false) is not FailedActionResult) // recalculate FOV and refresh UI
                OnPlayerAction();
        }
    }

    private void TickNewRound() // let all entities take their turns and be ticked
    {
        long startTime = Stopwatch.GetTimestamp(); // get time for perfomance debugging

        foreach (Entity entity in CurrentLevel!.Entities)
        {
            if (entity != Player && entity.Ai != null)
            {
                entity.Body?.Tick(entity, CurrentLevel!); // tick for regen until all components get ticked at the start of their turn

                entity.Ai!.Energy += 100; // add their energy
                while (entity.Ai.Energy > 0)
                    TickEntity(entity);
            }
        }
        _ticks++;

        System.Console.WriteLine($"Round {_ticks}\nElasped turn time: {Stopwatch.GetElapsedTime(startTime)}"); // print out time it took for round to process for turn time

        Player!.Body?.Tick(Player, CurrentLevel!); // tick for regen until all components get ticked at the start of their turn
        Player.Ai!.Energy += 100; // add player energy
        OnPlayerAction(); // recalculate FOV and refresh UI
        CurrentState = GameState.PLAYER_TURN;
    }

    private void OnPlayerAction() // runs when player does something and and the end of a round. Used for LOS and UI refreshes
    {
        LOSSystem.CalculateLos(Player!.Position!.Cords, CurrentLevel!); 
    }

    private ActionResult TickEntity(Entity entity, bool endTurnOnFail = true) // performs a single entity's turn
    {
        EntityAction action = entity.Ai!.Turn(entity);
        return EntityPerformAction(entity, action, endTurnOnFail);
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
        Engine.Instance!.ScreenManager.PlayerHealthDisplay.SetLimbs(Player.Body!); // temp stuff for getting hp set up
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