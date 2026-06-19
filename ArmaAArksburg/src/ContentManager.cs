public sealed class ContentManager
{
    public Dictionary<int, TileData> TilePallete {get; private set;} = [];

    public ContentManager()
    {
        TilePallete.Add(0, new TileData() // empty tile
        {
            Appearance = new ColoredGlyph(Color.Transparent, Color.Black, ' ')
        }
        );
        TilePallete.Add(1, new TileData() // wall
        {
            Appearance = new ColoredGlyph(Color.White, Color.Red, '#'),
            Solid = true
        })
        ;
        TilePallete.Add(2, new TileData() // water
        {
            Appearance = new ColoredGlyph(Color.White, Color.Blue, ' '),
            Solid = false,
            MoveCost = 150
        }
        );
    }
}