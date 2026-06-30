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
        // cancel if it is out of bounds
        if (!surface.Area.Contains(_position.Cords))
            return;

        surface[_position.Cords].CopyAppearanceFrom(_render.Appearance);
    }

    public EntityRenderable(PositionComponent position, RenderComponent render, int priority)
    {
        _position = position;
        _render = render;
        Priority = priority;
    }
}