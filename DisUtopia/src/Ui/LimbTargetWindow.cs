using ColorMine.ColorSpaces;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

public sealed class LimbTargetWindow : UiWindow
{
    private const int WIDTH = 40;

    private BodyComponent _body;
    private List<LimbData> _limbs;
    private ListBox _limbList;
    private Action<LimbData> _action;
    private readonly bool _goBackToPlayerState; // whether the game manager state should go back to PLAYER_TURN when the window is closed. Use when other ui opens this menu

    public static LimbTargetWindow Create(BodyComponent body, Action<LimbData> action, string windowTitle = "Select a limb", bool handleQuitting = true, bool goBackToPlayerState = true, UiWindow? absentParentConsole = null) // use to create limb windows
    {
        List<LimbData> limbs = [];
        BodyComponent.GetAllLimbs(limbs, body.RootLimb);
        Engine.Instance!.GameManager.CurrentState = GameManager.GameState.UI;

        LimbTargetWindow window = new LimbTargetWindow(body, limbs, action, windowTitle, goBackToPlayerState){HandleQuitting = handleQuitting, AbsentParentConsole = absentParentConsole};
        Engine.Instance!.ScreenManager.Children.Add( window );
        return window;
    }

    private void OnItemExecuted(object? sender, ListBox.SelectedItemEventArgs e)
    {
        SelectLimb((LimbData)e.Item!);
    }

    public override bool ProcessKeyboard(Keyboard keyboard) // keyboard controls
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            Close();
            return true;
        }
        if (keyboard.IsKeyPressed(Keys.Up))
        {
            _limbList.SelectedIndex = Math.Max(0, _limbList.SelectedIndex - 1);
            return true;
        }
        if (keyboard.IsKeyPressed(Keys.Down))
        {
            _limbList.SelectedIndex = Math.Min(_limbList.Items.Count - 1, _limbList.SelectedIndex + 1);
            return true;
        }
        if (keyboard.IsKeyPressed(Keys.Enter) && _limbList.SelectedItem != null) // force selection
        {
            SelectLimb((LimbData)_limbList.SelectedItem);
            return true;
        }
        return base.ProcessKeyboard(keyboard);
    }

    private void SelectLimb(LimbData limb)
    {
        _action.Invoke(limb);
        Close();
    }

    private LimbTargetWindow(BodyComponent body, List<LimbData> limbs, Action<LimbData> action, string windowTitle, bool goBackToPlayerState) : base(WIDTH, limbs.Count + 2, goBackToPlayerState) // use the create function not this one to instantiate the object
    {
        IsFocused = true;
        IsExclusiveMouse = true;

        // set basic properties
        Position = (Game.Instance.ScreenCellsX / 2 - WIDTH / 2, Game.Instance.ScreenCellsY - Height);
        _body = body;
        _limbs = limbs;
        _action = action;
        _goBackToPlayerState = goBackToPlayerState;

        // set up limb list
        _limbList = new(Width - 2, Height - 2)
        {
            Position = (1, 1)
        };

        foreach (LimbData limb in _limbs)
        {
            _limbList.Items.Add( limb );
        }

        _limbList.SelectedItemExecuted += OnItemExecuted;
        _limbList.SingleClickItemExecute = true;
        
        Controls.Add(_limbList);
        _limbList.UpdateAndRedraw(new());

        // title
        Label title = new(WIDTH);
        title.Alignment = HorizontalAlignment.Center;
        title.DisplayText = windowTitle;

        Controls.Add(title);
        title.UpdateAndRedraw(new());
    }
}