using SadConsole.Configuration;

Settings.WindowTitle = "Armageddon at Arksburg";
Settings.ResizeMode = Settings.WindowResizeOptions.Fit;

Builder
    .GetBuilder()
    .SetWindowSizeInCells(120, 38)
    .ConfigureFonts(true)
    .SetStartingScreen<RootConsole>()
    .IsStartingScreenFocused(true)
    .Run();

public class RootConsole: ScreenObject
{
    public GameWorldSurface worldSurface {get; private set;}

    public RootConsole()
    {
        Game.Instance.MonoGameInstance.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 144.0); // increase fps to 144

        worldSurface = new();
        Children.Add(worldSurface);

        Engine.Console = this;
        Engine.Keyboard = Game.Instance.Keyboard;
        Engine.Rng = new();

        Engine.Map = new Map(120, 38);

        Engine.Player = new Actor(new ColoredGlyph(Color.LimeGreen, Color.Transparent, '@'), (0,0));
        Engine.Player.Solid = true;
        Engine.Player.DrawPriority = 100;
        Engine.Player.Ai = new PlayerAi(Engine.Player)
        {
            Speed = 50
        };
        Engine.Map.AddActor(Engine.Player);

        for (int i = 0; i < 1; i++)
        {
            Actor actor = new(new ColoredGlyph(Color.LightSalmon, Color.Transparent, '@'), (20,20));
            actor.Solid = true;
            actor.Ai = new MobAi(actor);
            Engine.Map.AddActor(actor);
        }

    }

    public override void Update(TimeSpan delta)
    {
        Engine.Tick();
        Engine.Redraw();
    }
}

// responsible for drawing the game world
public class GameWorldSurface : ScreenSurface
{
    public GameWorldSurface() : base(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY - 5)
    {
        UseMouse = false;
        UseKeyboard = false;

        Surface.DefaultBackground = Color.AnsiMagentaBright;
    }
}