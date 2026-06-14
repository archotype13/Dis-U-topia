public class Level
{
    public struct Tile(int id)
    {
        public int Id = id;
    }

    public int Width;
    public int Height;
    private Tile[] Tiles;
    public List<Entity> Entities = [];


    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x + y*Width];
    }

    public Tile GetTileAt(Point point) { return GetTileAt(point.X, point.Y); }

    public void SetTileAt(int x, int y, Tile tile)
    {
        Tiles[x + y*Width] = tile;
    }

    public void SetTileAt(Point point, Tile tile) { SetTileAt(point.X, point.Y, tile); }

    public Level(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[width * height];

        Entities.Add(new Entity()
        {
            Name = "John Doe",
            Position = new(width / 2, height / 2),
            Render = new(new(Color.Yellow, Color.Transparent, '@')),
            Ai = new DrunkAiComponent()
        }
        );

        Engine.Instance!.GameManager.Player = new Entity()
        {
            Name = "Jane Doe",
            Position = new(6, 5),
            Render = new(new(Color.Purple, Color.Transparent, '@')),
            Ai = new PlayerAiComponent()
            {
                Speed = 50
            }
        };

        Entities.Add(Engine.Instance!.GameManager.Player);
    }
}