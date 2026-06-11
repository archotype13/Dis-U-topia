public class GameManager
{
    public Level? CurrentLevel;


    public void Update(TimeSpan delta)
    {
        if (CurrentLevel != null)
        {
            Engine.Instance!.ScreenManager.WorldView.RedrawLevel(CurrentLevel);
        }
    }
}