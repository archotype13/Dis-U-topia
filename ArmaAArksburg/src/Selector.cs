using SadConsole.Input;

public class Selector
{
    public virtual bool Select() // returns true if the selector has finished. This is when a valid selection has been entered or the selection is canceled
    {
        return true;
    }
}

public sealed class DirectionSelector : Selector
{
    private Action<Point> _action;
    private bool _first = true;
    public override bool Select()
    {
        if (_first)
        {
            _first = false;
            Engine.Instance!.ScreenManager.Log.LogMessage("Choose a direction\nPress escape to cancel");
        }

        // directional input
        Point? dir = null;
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad8) )
            dir = (0, -1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad9) )
            dir = (1, -1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad6) )
            dir = (1, 0);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad3) )
            dir = (1, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad2) )
            dir = (0, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad1) )
            dir = (-1, 1);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad4) )
            dir = (-1, 0);
        if ( Engine.Keyboard.IsKeyPressed(Keys.NumPad7) )
            dir = (-1, -1);

        // canceling
        if ( Engine.Keyboard.IsKeyPressed(Keys.Escape) )
        {
            Engine.Instance!.ScreenManager.Log.LogMessage("[c:r f:Gray]Nevermind");
            return true;
        }
        
        if (dir is Point point)
        {
            _action.Invoke(point);
            return true;
        }
        return false;

    }

    public DirectionSelector(Action<Point> action)
    {
        _action = action;
    }
}