using SadConsole.UI;
using SadConsole.UI.Controls;

public sealed class InventoryWindow : ControlsConsole
{
    private const int WIDTH = 40;
    private const int HEIGHT = 30;

    public override void Update(TimeSpan delta)
    {
        if (Engine.Keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
            Close();
        base.Update(delta);
    }

    public void Close()
    {
        Engine.Instance!.ScreenManager.Children.Remove(this);
        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.PLAYER_TURN;
    }

    public InventoryWindow() : base(WIDTH, HEIGHT)
    {
        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.UI;

        Position = (Game.Instance.ScreenCellsX / 2 - Width / 2, Game.Instance.ScreenCellsY / 2 - Height / 2);

        // create selection window
        EntitySelectionWindow selectionWindow = new(0, 0, WIDTH, HEIGHT, Engine.Instance!.GameManager.Player!.Inventory!.Items, "Inventory", false);
        Children.Add(selectionWindow);

        // add weight label
        Label weight = new(WIDTH - 2);
        weight.DisplayText = $"Weight: {Engine.Instance!.GameManager.Player!.Inventory!.CurrentWeight} / {Engine.Instance!.GameManager.Player!.Inventory!.MaxWeight}";
        weight.Position = (1, HEIGHT - 1);
        weight.Alignment = HorizontalAlignment.Right;
        // panel.Add(weight);
        selectionWindow.Controls.Add(weight);
        weight.UpdateAndRedraw(new());
        // panel.UpdateAndRedraw(new());
    }

    public static void Create() // automatically adds and creates and instance of the inventory window
    {
        Engine.Instance!.ScreenManager.Children.Add(new InventoryWindow() );
    }
}