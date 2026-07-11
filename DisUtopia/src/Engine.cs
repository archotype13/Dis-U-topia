using SadConsole.Input;

public sealed class Engine : ScreenObject
{
    public static Engine? Instance {get; private set;}
    public static Keyboard Keyboard {get; private set;} = Game.Instance.Keyboard;
    public static Random Rng {get; private set;}= Random.Shared;
    public ContentManager ContentManager {get; private set;} = new();
    public ScreenObjectManager ScreenManager {get; private set;} = new();
    public GameManager GameManager {get; private set;} = new();

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
    }

    public Engine()
    {
        Instance = this;
        Game.Instance.Screen = this; // make the current screen
        Children.Add(ScreenManager);
        Children.Add(GameManager);

        if (SaveManager.SaveExists())
            SaveManager.LoadGame();
        else
        {
            GameManager.CurrentLevel = new(ScreenManager.WorldView.Width, ScreenManager.WorldView.Height);
            GameManager.CurrentLevel.Init();
        }
    }
}