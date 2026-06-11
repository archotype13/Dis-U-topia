using SadConsole.Configuration;

Settings.WindowTitle = "Armageddon at Arksburg";

Builder
    .GetBuilder()
    .SetWindowSizeInCells(ScreenObjectManager.WINDOW_WIDTH, ScreenObjectManager.WINDOW_HEIGHT)
    .ConfigureFonts(true)
    .UseDefaultConsole()
    .OnStart(Startup)
    .Run();

static void Startup(object? sender, GameHost host)
{
    // create a new engine
    new Engine();
}
