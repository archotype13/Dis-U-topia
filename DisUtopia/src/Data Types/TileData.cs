public sealed class TileData()
{
    public string Name = "tile";
    public ColoredGlyph Appearance = new(Color.Transparent, Color.Purple, '?');
    public bool Solid = false;
    public bool Opaque = false; // determines whether player can see through the tile
    public int MoveCost = 100;
    public bool Invincible = false; // cannot be targeted
    public bool RequiresForced = true; // requires being forced to be attacked
    public int Hp = 1;
    public int Av = 0;
    public int DestroysInto = 0; // id of the tile this turns into when it is destroyed
}