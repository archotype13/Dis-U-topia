public interface IRenderable
{
    public int Priority {get; set;}
    public void Render(ICellSurface surface);
}

// used for drawing entities with basic render components
public sealed class EntityRenderable : IRenderable
{
    private PositionComponent _position;
    private RenderComponent _render;
    public int Priority {get; set;}
    public void Render(ICellSurface surface)
    {
        // cancel if it is out of bounds or isn't in los or explored
        Level.Tile tile = Engine.Instance!.GameManager.CurrentLevel!.GetTileAt(_position.Cords);
        if (!surface.Area.Contains(_position.Cords) || ( !tile.IsVisible && !(tile.IsExplored && _render.DrawWhenExplored) && !Engine.Instance!.GameManager.IgnoreLOS) )
            return;

        surface[_position.Cords].CopyAppearanceFrom(_render.Appearance);

        if (tile.IsExplored && !tile.IsVisible) // darken if explored and not visible
        {
            if (_render.Appearance.Background != Color.Transparent)
                surface[_position.Cords].Background = surface[_position.Cords].Background * GeneralConstants.EXPLORED_DARKEN;
            if (_render.Appearance.Foreground != Color.Transparent)
                surface[_position.Cords].Foreground = surface[_position.Cords].Foreground * GeneralConstants.EXPLORED_DARKEN;
        }
            
    }

    public EntityRenderable(PositionComponent position, RenderComponent render, int priority)
    {
        _position = position;
        _render = render;
        Priority = priority;
    }
}