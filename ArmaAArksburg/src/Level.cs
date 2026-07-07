public sealed class Level : Persistant
{
    public struct Tile(int id, int hp)
    {
        public int Id = id;
        public int Hp = hp;
    }

    public int Width {get; private set;} = 0;
    public int Height {get; private set;} = 0;
    private Tile[] Tiles {get; set;} = [];
    public List<Entity> Entities {get; private set;} = [];
    private List<Entity> DeletionQueue {get; set;} = [];
    private List<Entity> AddQueue {get; set;} = [];
    public AStarGrid Grid {get; private set;}


    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x + y*Width];
    }

    public Tile GetTileAt(Point point) { return GetTileAt(point.X, point.Y); }

    public void SetTileAt(int x, int y, int tileId, int forcedHp = -1) // if forced hp is not -1, hp will be set to the forced hp value
    {
        // get rid of old tile data on the astar grid
        TileData oldTile = Engine.Instance!.ContentManager.TilePallete[Tiles[x + y*Width].Id];
        Grid!.SetCellSolid(x, y, !oldTile.Solid);
        Grid.SetCellMoveCost(x, y, -oldTile.MoveCost);
        // add new data
        TileData newTileData = Engine.Instance!.ContentManager.TilePallete[tileId];
        Tile newTile = new(tileId, (forcedHp == -1)? newTileData.Hp : forcedHp);
        Tiles[x + y*Width] = newTile;
        Grid.SetCellSolid(x, y, newTileData.Solid);
        Grid.SetCellMoveCost(x, y, newTileData.MoveCost);
    }

    public void SetTileAt(Point point, int tileId) { SetTileAt(point.X, point.Y, tileId); }

    public void SetTileHealth(Point point, int newHealth)
    {
        Tiles[point.X + point.Y*Width].Hp = newHealth;
    }

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

    public void AddEntity(Entity entity, bool init = false) // change with a better system later
    {
        AddQueue.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entity.RemoveFromLevel(this);
        DeletionQueue.Add(entity);
    }

    public void FlushEntities() // clears out all of the delected enemies from the queue and adds in new ones
    {
        foreach (Entity entity in DeletionQueue)
        {
            Entities.Remove(entity);
        }
        DeletionQueue.Clear();
        foreach (Entity entity in AddQueue)
        {
            entity.AddToLevel(this);
            Entities.Add(entity);
        }
        AddQueue.Clear();
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
                            Render = new(new(Color.Brown, Color.Transparent, '+'), -1),
                            Destructible = new()
                            {
                                RequiresForced = true
                            },
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
                // SetTileAt(x, y, 0);
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
                Render = new(new(Color.Yellow, Color.Transparent, '@'), 1),
                Body = new()
                {
                    Corpse = new(){CorpseName = "John corpse", Appearance = new(Color.Red, Color.Transparent, '%')},
                    RootLimb = new()
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
                                Hp = 30
                            },
                            new()
                            {
                                Name = "right arm",
                                MaxHp = 30,
                                Hp = 30,
                            },
                            new()
                            {
                                Name = "left leg",
                                MaxHp = 30,
                                Hp = 30,
                            },
                            new()
                            {
                                Name = "right leg",
                                MaxHp = 30,
                                Hp = 30,
                            }
                        ]
                    }
                },
                Attack = new()
                {
                    Attack = new(){MinDamage = 1, MaxDamage = 4}
                },
                Ai = new BasicAiComponent()
                {
                    Speed = 100,
                    Energy = 100
                }
            }
            );
        }

        Engine.Instance!.GameManager.Player = new Entity()
        {
            Name = "Player",
            Position = new(3, 3)
            {
                Solid = true
            },
            Render = new(new(Color.Purple, Color.Transparent, '@'), 100),
            Body = new()
            {
                DvMod = 5,
                Corpse = new(){CorpseName = "your cadaver", Appearance = new(Color.Red, Color.Transparent, '%')},
                RootLimb = new()
                {
                    Name = "torso",
                    MaxHp = 40,
                    Hp = 40,
                    Av = 12,
                    Dv = -1,
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
                            Hp = 30
                        },
                        new()
                        {
                            Name = "right arm",
                            MaxHp = 30,
                            Hp = 30,
                        },
                        new()
                        {
                            Name = "left leg",
                            MaxHp = 30,
                            Hp = 30,
                        },
                        new()
                        {
                            Name = "right leg",
                            MaxHp = 30,
                            Hp = 30,
                        }
                    ]
                }
            },
            Attack = new()
            {
                Attack = new(){MinDamage = 1, MaxDamage = 6, Ap = 2, ToHit = 2}
            },
            Ai = new PlayerAiComponent()
            {
                Speed = 100
            }
        };

        AddEntity(Engine.Instance!.GameManager.Player);
        Engine.Instance!.ScreenManager.PlayerHealthDisplay.SetLimbs(Engine.Instance!.GameManager.Player.Body);

        FlushEntities();
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
                Tile tile = GetTileAt(x, y);
                writer.Write(tile.Id); // write tile id
                writer.Write(tile.Hp); // write tile hp
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
                SetTileAt(x, y, reader.ReadInt32(), reader.ReadInt32()); // set tile id
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

        FlushEntities();
    }

}