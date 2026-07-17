using SadConsole.UI;

public class UiWindow : ControlsConsole
{
    private readonly bool _goBackToPlayerState; // whether the game manager state should go back to PLAYER_TURN when the window is closed. Use when other ui opens this menu
    public ScreenObject? AbsentParentConsole; // since all of these windows are children of the root screen, this variable keeps track of the console that created it for exclusive mouse stuff
    public event Action? OnClosed;

    public override void Update(TimeSpan delta)
    {
        if ( Engine.Keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape) && UseKeyboard)
            Close();

        base.Update(delta);
    }

    public void Close()
    {
        Engine.Instance!.ScreenManager.Children.Remove(this);
        IsExclusiveMouse = false;
        if (_goBackToPlayerState)
            Engine.Instance!.GameManager.CurrentState = GameManager.GameState.PLAYER_TURN;
        if (AbsentParentConsole != null)
        {
            AbsentParentConsole.IsExclusiveMouse = true;
        }
            
        OnClosed?.Invoke();
    }

    public UiWindow(int width, int height, bool goBackToPlayerState) : base(width, height)
    {
        _goBackToPlayerState = goBackToPlayerState;
    }
}