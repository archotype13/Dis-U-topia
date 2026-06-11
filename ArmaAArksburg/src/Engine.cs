using System.Diagnostics;
using System.Net.Cache;
using Newtonsoft.Json;

public class Engine : ScreenObject
{
    public static Engine? Instance {get; private set;}
    public ContentManager ContentManager {get; private set;} = new();
    public ScreenObjectManager ScreenManager {get; private set;} = new();
    public GameManager GameManager {get; private set;} = new();

    public override void Update(TimeSpan delta)
    {
        GameManager.Update(delta);
        base.Update(delta);
    }

    public Engine()
    {
        Instance = this;
        Game.Instance.Screen = this; // make the current screen
        Children.Add(ScreenManager);

        GameManager.CurrentLevel = new(ScreenManager.WorldView.Width, ScreenManager.WorldView.Height);

        ScreenManager.Log.LogMessage("Hi there! this is a [c:r f:Yellow]SUPPPPERRRRRRRR[c:u] duper long log message!");
        ScreenManager.Log.LogMessage("Hi there! this is a [c:r f:Yellow]SUPPPPERRRRRRRR[c:u] duper long log message!");
    }
}