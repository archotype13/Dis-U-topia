public sealed class WorldViewport : Console
{
    public List<Point> debugDrawPoints = [];
    public void RedrawLevel(Level level)
    {
        for (int y = 0; y < level.Height; y ++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                Surface[x, y].CopyAppearanceFrom(Engine.Instance!.ContentManager.TilePallete[level.GetTileAt(x, y).Id].Appearance);
            }
        }

        foreach (Entity entity in level.Entities)
        {
            if (entity.Position != null && entity.Render != null)
            {
                if (Surface.Area.Contains(entity.Position.Cords))
                    entity.Render.Draw(entity.Position.Cords, Surface);
            }
        }

        foreach (Point point in debugDrawPoints)
        {
            Surface[point].Background = Color.Magenta;
        }

        IsDirty = true;
    }


    public WorldViewport(int x, int y, int sizeX, int sizeY) : base(sizeX, sizeY)
    {
        Position = (x, y);
    }
}