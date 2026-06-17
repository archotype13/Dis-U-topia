public class ContentManager
{
    public Dictionary<int, TileData> TilePallete {get; private set;} = [];

    public ContentManager()
    {
        TilePallete.Add(0, new TileData()
        {
            Appearance = new ColoredGlyph(Color.Transparent, Color.Blue, ' ')
        }
        );
        TilePallete.Add(1, new TileData()
        {
            Appearance = new ColoredGlyph(Color.White, Color.Red, '#'),
            Solid = true
        }
        );
    }
}