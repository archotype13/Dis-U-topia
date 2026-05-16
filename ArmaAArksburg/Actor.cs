using SadConsole.Quick;

public class Actor
{
    public Point Position;
    public bool Solid = false;
    public bool QueuedForDeletion = false;
    public ColoredGlyph Appearance;
    public int DrawPriority = 0;
    public Ai? Ai;

    public Actor(ColoredGlyph appearance, Point position)
    {
        Appearance = appearance;
        Position = position;
    }

    public void Draw(ICellSurface surface)
    {
        if (surface.Area.Contains(Position))
        {
            Appearance.CopyAppearanceTo(surface[Position]);
            // only add a background if the actor has one
            if (Appearance.Background == Color.Transparent)
                surface[Position].Background = Engine.Map!.GetTileAt(Position).Appearance.Background;
        }
    }
    
    public bool Move(Point newPosition, bool collision = true) // used to move actors to a new position (used to handle collision and traps) Set collision to false to not care if the new position is solid
    {
        if (Engine.Map!.IsInBounds(newPosition) && collision && !Engine.Map.IsSolidAt(newPosition))
        {
            // target tile is open for movement
            Point oldPosition = Position;
            Position = newPosition;

            // update solids
            Engine.Map.UpdateSolid(oldPosition);
            Engine.Map.UpdateSolid(newPosition);
            return true;
        }
        return false;
    }
}