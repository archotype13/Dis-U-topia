using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

public sealed class InventoryWindow : UiWindow
{
    private const int WIDTH = 40;
    private const int HEIGHT = 30;
    private readonly EntitySelectionWindow _selectionWindow;

    public InventoryWindow() : base(WIDTH, HEIGHT, true)
    {
        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.UI;

        Position = (Game.Instance.ScreenCellsX / 2 - Width / 2, Game.Instance.ScreenCellsY / 2 - Height / 2);
        IsFocused = true;

        // create selection window
        _selectionWindow = new(0, 0, WIDTH, HEIGHT, Engine.Instance!.GameManager.Player!.Inventory!.Items, "Inventory", false);
        _selectionWindow.List.SelectedItemExecuted += (s, a) => 
        {
            UseKeyboard = false;
            EntityExaminationWindow entityExaminationWindow = EntityExaminationWindow.Create((Entity)a.Item!, false);
            entityExaminationWindow.OnClosed += () => { UseKeyboard = true; };
        };
        // redraw when the player's inventory changes
        Engine.Instance!.GameManager.Player!.Inventory!.OnChanged += (s, a) => {_selectionWindow.List.Items.Remove(a!.Entity);};

        Children.Add(_selectionWindow);

        // add weight label
        Label weight = new(WIDTH - 2);
        weight.DisplayText = $"Weight: {Engine.Instance!.GameManager.Player!.Inventory!.CurrentWeight} / {Engine.Instance!.GameManager.Player!.Inventory!.MaxWeight}";
        weight.Position = (1, HEIGHT - 1);
        weight.Alignment = HorizontalAlignment.Right;
        _selectionWindow.Controls.Add(weight);
        weight.UpdateAndRedraw(new());
    }

    public static void Create() // automatically adds and creates and instance of the inventory window
    {
        Engine.Instance!.ScreenManager.Children.Add(new InventoryWindow() );
    }
}