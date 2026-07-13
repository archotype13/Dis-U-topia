using SadConsole.UI;
using SadConsole.UI.Controls;

public class EntitySelectionWindow : ControlsConsole
{
    public readonly ListBox List;
    private readonly bool HandleQuiting;
    private readonly bool GoBackToPlayerState;

    public override void Update(TimeSpan delta)
    {
        if (HandleQuiting && Engine.Keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
            Close();
        base.Update(delta);
    }

    public void Close()
    {
        Engine.Instance!.ScreenManager.Children.Remove(this);
        if (GoBackToPlayerState)
            Engine.Instance!.GameManager.CurrentState = GameManager.GameState.PLAYER_TURN;
    }

    public EntitySelectionWindow(int x, int y, int width, int height, List<Entity> entities, string title, bool handleQuiting, bool goBackToPlayerState = true) : base(width, height)
    {
        Position = (x, y);
        HandleQuiting = handleQuiting;
        GoBackToPlayerState = goBackToPlayerState;

        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.UI;

        // add panel
        Panel panel = new(width, height);
        panel.DrawBorder = true;
        Controls.Add(panel);
        
        // add title
        Label titleLabel = new(title);
        titleLabel.Position = (1, 0);
        panel.Add(titleLabel);

        // add entity list
        List = new(width - 2, height - 2);
        List.Position = (1, 1);
        List.IsScrollBarVisible = true;
        // add items
        foreach (Entity entity in entities)
            List.Items.Add(entity);
        List.UpdateAndRedraw(new());
        // had to manually do selection due to weird behaviors
        List.MouseButtonClicked += (s, a) => {if (a.OriginalMouseState.Mouse.RightClicked == false && List.ItemIndexMouseOver != -1) List.SelectedIndex = List.ItemIndexMouseOver;};
        panel.Add(List);

        panel.UpdateAndRedraw(new());
    }

    public static EntitySelectionWindow Create(int x, int y, int width, int height, List<Entity> entities, string title, bool goBackToPlayerState) // automatically adds and creates and instance of the selection window
    {
        EntitySelectionWindow selectionWindow = new EntitySelectionWindow(x, y, width, height, entities, title, true, goBackToPlayerState);
        Engine.Instance!.ScreenManager.Children.Add( selectionWindow );
        return selectionWindow;
    }
}