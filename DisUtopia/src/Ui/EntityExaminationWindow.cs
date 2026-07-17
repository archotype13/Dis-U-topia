using System.Runtime.InteropServices;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

public sealed class EntityExaminationWindow : ControlsConsole
{
    private const int WIDTH = 40;
    private readonly bool _goBackToPlayerState; // whether the game manager state should go back to PLAYER_TURN when the window is closed. Use when other ui opens this menu
    public ScreenObject? AbsentParentConsole; // since all of these windows are children of the root screen, this variable keeps track of the console that created it for exclusive mouse stuff
    private readonly Entity _entity;
    public event Action? OnClosed;

    private readonly Dictionary<Keys, Button> ButtonShortcuts = [];

    public override void Update(TimeSpan delta)
    {
        if ( Engine.Keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape) && UseKeyboard)
            Close();

        base.Update(delta);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if ( keyboard.KeysPressed.Count > 0 &&  ButtonShortcuts.TryGetValue(keyboard.KeysPressed.First().Key, out Button? button) && button != null )
        {
            button.InvokeClick();
            return true;
        }
        return base.ProcessKeyboard(keyboard);
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

    public EntityExaminationWindow(Entity entity, int height, bool goBackToPlayerState) : base(WIDTH, height)
    {
        _goBackToPlayerState = goBackToPlayerState;
        _entity = entity;

        IsFocused = true;
        UseKeyboard = true;
        IsExclusiveMouse = true; // force this to be the only thing using the mouse

        Position = ( (Game.Instance.ScreenCellsX - Width) / 2, (Game.Instance.ScreenCellsY - Height) / 2);

        Panel panel = new(Width, Height);
        panel.DrawBorder = true;

        int y = 1;

        // start adding information
        Label title = new(entity.Name);
        title.Position = (Width / 2 - entity.Name.Length / 2, y);
        panel.Add(title);
        title.UpdateAndRedraw(new());
        y++;

        // buttons for relevant actions
        if (entity.Item != null)
        {
            if (entity.Item.Consumable != null) // consumable button
            {
                Button consumableButton = new("C) Consume");

                consumableButton.UpdateAndRedraw(new());
                System.Console.WriteLine(consumableButton.MouseArea.Width);

                consumableButton.Position = ((Width - consumableButton.MouseArea.Width) / 2, y);
                // set up action
                consumableButton.Click += (s, a) =>
                {
                    System.Console.WriteLine("Consumed");
                    Close();
                };

                ButtonShortcuts.Add(Keys.C, consumableButton);
                panel.Add(consumableButton);

                y++;
            }
        }

        Controls.Add(panel);
    }

    public static EntityExaminationWindow Create(Entity entity, bool goBackToPlayerState)
    {
        // get the height to include all the buttons
        int y = 3;

        // item data info
        if (entity.Item != null)
        {
            if (entity.Item.Consumable != null) y++;
        }

        EntityExaminationWindow examinationWindow = new EntityExaminationWindow(entity, y, goBackToPlayerState);
        Engine.Instance!.ScreenManager.Children.Add( examinationWindow );
        return examinationWindow;
    }
}