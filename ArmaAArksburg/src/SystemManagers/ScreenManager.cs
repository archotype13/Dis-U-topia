public class ScreenObjectManager : ScreenObject // organizes sub-windows for the game's view
{
    // window constants
    public const int WINDOW_WIDTH = 120;
    public const int WINDOW_HEIGHT = 30;
    // world viewport constants
    private const int WORLDVIEW_X = 0;
    private const int WORLDVIEW_Y = 0;
    private const int WORLDVIEW_WIDTH = 90;
    private const int WORLDVIEW_HEIGHT = WINDOW_HEIGHT;
    // health ui constants
    private const int HEALTH_X = WORLDVIEW_WIDTH;
    private const int HEALTH_Y = 1;
    private const int HEALTH_WIDTH = WINDOW_WIDTH - WORLDVIEW_WIDTH;
    private const int HEALTH_HEIGHT = 7;
    // message log constants
    private const int LOG_X = WORLDVIEW_WIDTH;
    private const int LOG_Y = HEALTH_HEIGHT;
    private const int LOG_WIDTH = WINDOW_WIDTH - WORLDVIEW_WIDTH;
    private const int LOG_HEIGHT = WINDOW_HEIGHT - HEALTH_HEIGHT + HEALTH_Y;

    // properties
    public WorldViewport WorldView {get; private set;} = new(WORLDVIEW_X, WORLDVIEW_Y, WORLDVIEW_WIDTH, WORLDVIEW_HEIGHT);
    public HealthDisplay PlayerHealthDisplay {get; private set;} = new(HEALTH_X, HEALTH_Y, HEALTH_WIDTH, HEALTH_HEIGHT);
    public MessageLog Log {get; private set;} = new(LOG_X, LOG_Y, LOG_WIDTH, LOG_HEIGHT);

    public ScreenObjectManager()
    {
        // add the child screen objects
        Children.Add(WorldView);
        Children.Add(PlayerHealthDisplay);
        // WorldView.FillWithRandomGarbage(WorldView.Font);
        Children.Add(Log);
    }
}