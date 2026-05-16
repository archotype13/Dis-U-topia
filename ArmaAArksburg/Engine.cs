using System.Diagnostics;
using SadConsole.Input;

public static class Engine
{
    public static RootConsole? Console {get; set;}
    public static Map? Map {get; set;}
    public static Actor? Player {get; set;}
    public static Keyboard? Keyboard {get; set;}
    public static Random? Rng {get; set;}
    public static EngineStates CurrentState = EngineStates.PLAYER_TURN;
    public static List<Point> DebugHighlights = [];
    public enum EngineStates
    {
        PLAYER_TURN,
        NEW_TURN
    }

    public static void Tick()
    {
        if (Map == null)
            return;

        if (CurrentState == EngineStates.PLAYER_TURN)
        {
            TickActor(Player!);

            if (Player!.Ai!.Energy <= 0)
                CurrentState = EngineStates.NEW_TURN;
        }
        else if (CurrentState == EngineStates.NEW_TURN)
        {
            long startTime = Stopwatch.GetTimestamp(); // used to check how long the frame took to process

            foreach (Actor actor in Map.Actors)
            {
                if (!actor.QueuedForDeletion && actor != Player && actor.Ai != null)
                {
                    actor.Ai.GainEnergy();

                    // loop until actor has completed all available actions
                    while (actor.Ai.Energy > 0)
                    {
                        TickActor(actor);   
                    }
                }
            }

            // flush deleted entities
            Map.Flush();

            System.Console.WriteLine($"Delta time: {Stopwatch.GetElapsedTime(startTime)} ms"); // used for checking how long the turn took to process

            // make player gain their energy now to enable them to contantly tick through their own turn.
            CurrentState = EngineStates.PLAYER_TURN;
            Player!.Ai!.GainEnergy();
        }
    }

    private static void TickActor(Actor actor) // tick actor AI
    {
        Action action = actor.Ai!.TakeTurn();
        actor.Ai.Energy -= action.Perform();
    }

    public static void Redraw()
    {
        Dictionary<Point, int> drawPriority = [];

        if (Map != null)
        {
            Map.Draw(Console!.worldSurface.Surface);

            // draw actors
            foreach (Actor actor in Map.Actors)
            {
                if (drawPriority.TryGetValue(actor.Position, out int cellDrawPriority) && actor.DrawPriority <= cellDrawPriority) // check if the draw priority already has a register entity at that position with a higher priority
                    continue;

                actor.Draw(Console!.worldSurface.Surface);
                drawPriority[actor.Position] = actor.DrawPriority;
            }

            // debug hightlights
            foreach(Point point in DebugHighlights)
            {
                if (Console!.worldSurface.Surface.Area.Contains(point))
                    Console!.worldSurface.Surface[point].Background = Color.AnsiCyan;
            }
        }
        
        Console!.worldSurface.IsDirty = true;
    }

}