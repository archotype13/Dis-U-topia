public class ContentManager
{
    public Dictionary<int, TileData> TilePallete {get; private set;} = [];

    public ContentManager()
    {
        TilePallete.Add(0, new TileData(new ColoredGlyph(Color.MistyRose, Color.Blue, '#')));
    }
}