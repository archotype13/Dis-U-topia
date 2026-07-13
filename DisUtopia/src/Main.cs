using SadConsole.Configuration;

Settings.WindowTitle = "(Dis/U)topia";

Builder
    .GetBuilder()
    .SetWindowSizeInCells(ScreenObjectManager.WINDOW_WIDTH, ScreenObjectManager.WINDOW_HEIGHT)
    .ConfigureFonts(true)
    .UseDefaultConsole()
    .OnStart(Startup)
    .OnEnd(End)
    .Run();

static void Startup(object? sender, GameHost host)
{
    // set up constants
    GeneralConstants.Init();
    // create a new engine
    _ = new Engine();
}


static void End(object? sender, GameHost host)
{
    // Place your save states or resource cleanup code here
    System.Console.WriteLine("Main window is closing. Performing cleanup...");
    // manage saving
    if (Engine.Instance!.GameManager.CurrentState != GameManager.GameState.GAME_OVER)
        SaveManager.SaveGame();
}