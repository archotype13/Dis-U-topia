public sealed class ContentManager
{
    public Dictionary<int, TileData> TilePallete {get; private set;} = [];

    public ContentManager()
    {
        TilePallete.Add(0, new TileData() // empty tile
        {
            Name = "air",
            Appearance = new ColoredGlyph(Color.DarkGray, Color.Transparent, 9 + 15*16),
            Invincible = true
        }
        );
        TilePallete.Add(1, new TileData() // wall
        {
            Name = "concrete wall",
            Appearance = new ColoredGlyph(Color.Transparent, Color.LightGray, ' '),
            Solid = true,
            Hp = 50,
            Av = 25,
            DestroysInto = 0
        })
        ;
        TilePallete.Add(2, new TileData() // water
        {
            Name = "water",
            Appearance = new ColoredGlyph(Color.White, Color.Blue, '.'),
            Solid = false,
            MoveCost = 150,
            Invincible = true
        }
        );
    }
}