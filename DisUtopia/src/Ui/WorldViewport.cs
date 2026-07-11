public sealed class WorldViewport : Console
{
    private const float EXPLORED_DARKEN = 0.75f;
    private SortedDictionary<int, List<IRenderable>> Renderables = [];
    public List<Point> debugDrawPoints = [];
    public void RedrawLevel(Level level) // redraws the entire level
    {
        Surface.Clear();

        for (int y = 0; y < level.Height; y ++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                Level.Tile tile = level.GetTileAt(x, y);
                if (tile.IsVisible) // only draw visible tiles
                {
                    Surface[x, y].CopyAppearanceFrom(Engine.Instance!.ContentManager.TilePallete[tile.Id].Appearance);
                }
                else if (tile.IsExplored) // draw greyscaled for explored tiles
                {
                    ColoredGlyph appearance = (ColoredGlyph)Engine.Instance!.ContentManager.TilePallete[tile.Id].Appearance.Clone();
                    appearance.Background = GeneralConstants.GrayscaleColor(appearance.Background) * EXPLORED_DARKEN;
                    appearance.Foreground = GeneralConstants.GrayscaleColor(appearance.Foreground) * EXPLORED_DARKEN;
                    Surface[x, y].CopyAppearanceFrom(appearance);
                }
            }
        }

        // priority drawing for IRenderables
        foreach (int i in Renderables.Keys)
        {
            foreach (IRenderable renderable in Renderables[i])
            {
                renderable.Render(Surface);
            }
        }

        foreach (Point point in debugDrawPoints)
        {
            Surface[point].Background = Color.Magenta;
        }

        IsDirty = true;
    }

    public void AddRenderable(IRenderable renderable)
    {
        // create a list for the priority if there already isn't one
        if ( !Renderables.ContainsKey(renderable.Priority) )
            Renderables[renderable.Priority] = [];
        // actually add the IRenderable
        Renderables[renderable.Priority].Add(renderable);
    }
    public void RemoveRenderable(IRenderable renderable)
    {
        Renderables[renderable.Priority].Remove(renderable);
    }


    public WorldViewport(int x, int y, int sizeX, int sizeY) : base(sizeX, sizeY)
    {
        Position = (x, y);
    }
}