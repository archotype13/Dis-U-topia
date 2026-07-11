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
            // Appearance = new ColoredGlyph(Color.Transparent, Color.LightGray, ' '),
            Appearance = new ColoredGlyph(Color.Transparent, Color.DarkGray, ' '),
            Solid = true,
            Opaque = true,
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

    public static LimbData GetBasicHumanBody()
    {
        return new()
        {
            Name = "torso",
            MaxHp = 40,
            Hp = 40,
            Vital = true,
            ChildLimbs = 
            [
                new()
                {
                    Name = "head",
                    MaxHp = 30,
                    Hp = 30,
                    Vital = true
                },
                new()
                {
                    Name = "left arm",
                    MaxHp = 30,
                    Hp = 30,
                    ArmData = new() {Weight = 0.5f}
                },
                new()
                {
                    Name = "right arm",
                    MaxHp = 30,
                    Hp = 30,
                    ArmData = new() {Weight = 0.5f}
                },
                new()
                {
                    Name = "left leg",
                    MaxHp = 30,
                    Hp = 30,
                    LegData = new() {Weight = 0.5f}
                },
                new()
                {
                    Name = "right leg",
                    MaxHp = 30,
                    Hp = 30,
                    LegData = new() {Weight = 0.5f}
                }
            ]
        };
    }
}