public static class LOSSystem
{
    private const int ViewDistance = 20;

    private static int GetDistance(int x, int y)
    {
        return (int)Math.Sqrt(x*x + y*y);
    }
    public static void CalculateLos(Point origin, Level level)
    {
        level.ClearVisibility();
        LosAlgorithm los = new(level.IsOpaqueAt, level.SetVisible, GetDistance);
        los.Compute(origin, ViewDistance);
        
        // min view distance
        // for (int y = -ViewDistance - 1; y <= ViewDistance; y++)
        // {
        //     for (int x = -ViewDistance - 1; x <= ViewDistance + 1; x++)
        //     {
        //         if (level.IsInBounds(origin.X + x, origin.Y + y) && x*x + y*y <= ViewDistance*ViewDistance)
        //         {
        //             foreach (Point pos in Lines.GetBresenhamLine(origin, origin + (x, y)))
        //             {
        //                 level.SetVisible(pos.X, pos.Y, true);
        //                 level.SetExplored(pos.X, pos.Y, true);

        //                 if (Engine.Instance!.ContentManager.TilePallete[level.GetTileAt(pos.X, pos.Y).Id].Opaque) // break if an opaque tile has been reached
        //                     break;
        //             }
        //         }
        //     }
        // }
    }
        
}