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
    public AStarGrid Grid;


    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x + y*Width];
    }

    public Tile GetTileAt(Point point) { return GetTileAt(point.X, point.Y); }

    public void SetTileAt(int x, int y, int tileId)
    {
        TileData oldTile = Engine.Instance!.ContentManager.TilePallete[Tiles[x + y*Width].Id];
        Grid.SetCellSolid(x, y, oldTile.Solid);
        Grid.SetCellMoveCost(x, y, false, oldTile.MoveCost);
        TileData newTile = Engine.Instance!.ContentManager.TilePallete[tileId];
        Tiles[x + y*Width] = new Tile(tileId);
        Grid.SetCellSolid(x, y, newTile.Solid);
        Grid.SetCellMoveCost(x, y, true, newTile.MoveCost);
    }

    public void SetTileAt(Point point, int tileId) { SetTileAt(point.X, point.Y, tileId); }

    public bool IsInBounds(int x, int y) // returns true if the point is in bounds
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            return true;
        return false;
    }

    public bool IsInBounds(Point point) {return IsInBounds(point.X, point.Y);}

    public bool IsSolidAt(int x, int y)
    {
        AStarGrid.AstarTile tile = Grid.GetCell(x, y);
        if (tile.NSolids > 0)
            return true;
        return false;
    }

    public bool IsSolidAt(Point point) {return IsSolidAt(point.X, point.Y);}

    public void AddEntity(Entity entity) // change with a better system later
    {
        if (entity.Position != null && entity.Position.Solid)
        {
            Grid.SetCellSolid(entity.Position.Cords, true);
        }
        Entities.Add(entity);
    }

    public Level(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[width * height];
        Grid = new(Width, Height);

        // testing add solids
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 5 || y == 5)
                    SetTileAt(x, y, 2);
                else if (x % 3 == 0 && y % 3 == 0)
                    SetTileAt(x, y, 1);
                else
                    SetTileAt(x, y, 0);
            }
        }

        // add test entities
        for (int i = 0; i < 100; i++)
        {
            AddEntity(new Entity()
            {
                Name = "John Doe",
                Position = new(width / 2 + 1, height / 2)
                {
                    Solid = true
                },
                Render = new(new(Color.Yellow, Color.Transparent, '@')),
                Ai = new DrunkAiComponent()
                {
                    Speed = 100
                }
            }
            );
        }
        

        Engine.Instance!.GameManager.Player = new Entity()
        {
            Name = "Jane Doe",
            Position = new(6, 5)
            {
                Solid = true
            },
            Render = new(new(Color.Purple, Color.Transparent, '@')),
            Ai = new PlayerAiComponent()
            {
                Speed = 100
            }
        };

        AddEntity(Engine.Instance!.GameManager.Player);
    }
}