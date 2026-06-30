public sealed class ContentManager
{
    public Dictionary<int, TileData> TilePallete {get; private set;} = [];

    public ContentManager()
    {
        TilePallete.Add(0, new TileData() // empty tile
        {
            Appearance = new ColoredGlyph(Color.DarkGray, Color.Transparent, 9 + 15*16)
        }
        );
        TilePallete.Add(1, new TileData() // wall
        {
            Appearance = new ColoredGlyph(Color.Transparent, Color.LightGray, ' '),
            Solid = true
        })
        ;
        TilePallete.Add(2, new TileData() // water
        {
            Appearance = new ColoredGlyph(Color.White, Color.Blue, '.'),
            Solid = false,
            MoveCost = 150
        }
        );
    }
}