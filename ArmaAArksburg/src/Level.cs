using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

public sealed class Level : Persistant
{
    public struct Tile(int id)
    {
        public int Id = id;
    }

    public int Width {get; private set;} = 0;
    public int Height {get; private set;} = 0;
    private Tile[] Tiles {get; set;} = [];
    public List<Entity> Entities {get; private set;} = [];
    private List<Entity> DeletionQueue {get; set;} = [];
    public AStarGrid? Grid {get; private set;}


    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x + y*Width];
    }

    public Tile GetTileAt(Point point) { return GetTileAt(point.X, point.Y); }

    public void SetTileAt(int x, int y, int tileId)
    {
        // get rid of old tile data on the astar grid
        TileData oldTile = Engine.Instance!.ContentManager.TilePallete[Tiles[x + y*Width].Id];
        Grid!.SetCellSolid(x, y, oldTile.Solid);
        Grid.SetCellMoveCost(x, y, -oldTile.MoveCost);
        // add new data
        TileData newTile = Engine.Instance!.ContentManager.TilePallete[tileId];
        Tiles[x + y*Width] = new Tile(tileId);
        Grid.SetCellSolid(x, y, newTile.Solid);
        Grid.SetCellMoveCost(x, y, newTile.MoveCost);
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
        AStarGrid.AstarTile tile = Grid!.GetCell(x, y);
        if (tile.NSolids > 0)
            return true;
        return false;
    }

    public bool IsSolidAt(Point point) {return IsSolidAt(point.X, point.Y);}

    public List<Entity> GetEntitiesAt(Point point) // gets all the entities at a certain point
    {
        List<Entity> entities = [];
        foreach (Entity entity in Entities)
        {
            if (entity.Position != null && entity.Position.Cords == point)
                entities.Add(entity);
        }
        return entities;
    }

    public void AddEntity(Entity entity) // change with a better system later
    {
        entity.Position?.AddToLevel(entity, this);
        entity.Render?.AddToLevel(entity, this);
        entity.Door?.AddToLevel(entity, this);
        entity.Ai?.AddToLevel(entity, this);
        Entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        DeletionQueue.Add(entity);
        entity.Position?.RemoveFromLevel(entity, this);
        entity.Render?.RemoveFromLevel(entity, this);
        entity.Ai?.RemoveFromLevel(entity, this);
        entity.Door?.RemoveFromLevel(entity, this);
    }

    public void FlushDeletedEntities() // clears out all of the delected enemies from the queue
    {
        foreach (Entity entity in DeletionQueue)
        {
            Entities.Remove(entity);
        }
        DeletionQueue.Clear();
    }

    public void Init() // sets up the initial data of the level
    {
        // testing add solids
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if ( x % 6 == 0 || y % 6 == 0 ) // grid walls
                {
                    if ( (x % 3 == 0 && x % 6 != 0) || (y % 3 == 0 && y % 6 != 0 ) ) // place doors in centers of wall segments
                    {
                        SetTileAt(x, y, 0);
                        // add door
                        AddEntity(new Entity()
                        {
                            Name = "John Door",
                            Position = new(x, y)
                            {
                                Solid = true
                            },
                            Render = new(new(Color.Brown, Color.Transparent, '+'), 0),
                            Door = new()
                            {
                                OpenAppearance = new(Color.Brown, Color.Transparent, '-'),
                                ClosedAppearance = new(Color.Brown, Color.Transparent, '+')
                            }
                        }
                        );
                    }
                    else                                                       // place walls
                        SetTileAt(x, y, 1);
                }
                else
                    SetTileAt(x, y, 0);
            }
        }

        // add test entities
        for (int i = 0; i < 1; i++)
        {
            AddEntity(new Entity()
            {
                Name = "John Doe",
                Position = new(Width / 2 + 1, Height / 2)
                {
                    Solid = true
                },
                Render = new(new(Color.Yellow, Color.Transparent, '@'), 0),
                Ai = new BasicAiComponent()
                {
                    Speed = 100,
                    Energy = 100
                }
            }
            );
        }

        AddEntity(new Entity()
            {
                Name = "test",
                Position = new(6, 4)
                {
                    Solid = false
                },
                Render = new(new(Color.Yellow, Color.Transparent, '0'), 0),
            }
        );
        AddEntity(new Entity()
            {
                Name = "test2",
                Position = new(6, 4)
                {
                    Solid = false
                },
                Render = new(new(Color.Green, Color.Purple, '0'), -1),
            }
        );
        

        Engine.Instance!.GameManager.Player = new Entity()
        {
            Name = "Jane Doe",
            Position = new(6, 4)
            {
                Solid = true
            },
            Render = new(new(Color.Purple, Color.Transparent, '@'), 100),
            Ai = new PlayerAiComponent()
            {
                Speed = 100
            }
        };

        AddEntity(Engine.Instance!.GameManager.Player);
    }

    public Level(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[width * height];
        Grid = new(Width, Height);
    }

    public override void Save(BinaryWriter writer)
    {
        // save tile data
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                writer.Write(GetTileAt(x, y).Id); // write tile id
            }
        }

        // // save entities
        writer.Write(Entities.Count - 1); // ignore player. This is added by the game manager
        foreach (Entity entity in Entities)
        {
            if (entity != Engine.Instance!.GameManager.Player)
                entity.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        // load tile data
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                SetTileAt(x, y, reader.ReadInt32()); // set tile id
            }
        }

        // // load entities
        int entityCount = reader.ReadInt32();
        for (int i = 0; i < entityCount; i++)
        {
            Entity entity = new();
            entity.Load(reader);
            AddEntity(entity);
        }
    }

}