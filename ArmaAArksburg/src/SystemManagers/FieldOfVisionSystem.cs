public static class LOSSystem
{
    const int ViewDistance = 5;
    public static void CalculateLos(Point origin, Level level)
    {
        level.ClearVisibility();
        for (int y = -ViewDistance - 1; y <= ViewDistance; y++)
        {
            for (int x = -ViewDistance - 1; x <= ViewDistance + 1; x++)
            {
                if (level.IsInBounds(origin.X + x, origin.Y + y) && x*x + y*y <= ViewDistance*ViewDistance)
                {
                    foreach (Point pos in Lines.GetBresenhamLine(origin, origin + (x, y)))
                    {
                        level.SetVisible(pos.X, pos.Y, true);
                        level.SetExplored(pos.X, pos.Y, true);

                        if (Engine.Instance!.ContentManager.TilePallete[level.GetTileAt(pos.X, pos.Y).Id].Opaque) // break if an opaque tile has been reached
                            break;
                    }
                }
            }
        }
    }
}