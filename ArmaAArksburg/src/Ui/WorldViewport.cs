public class WorldViewport : Console
{
    
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
                entity.Render.Draw(entity.Position.Cords, Surface);
            }
        }
    }


    public WorldViewport(int x, int y, int sizeX, int sizeY) : base(sizeX, sizeY)
    {
        Position = (x, y);
    }
}