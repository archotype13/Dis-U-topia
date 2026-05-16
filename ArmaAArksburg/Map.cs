using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class Map
{
    public int Height {get; private set;}
    public int Width {get; private set;}
    // arrays quickly for checking if a cell is solid or not
    private bool[] solids;
    private Tile[] tiles;
    public List<Actor> Actors = []; 
    private List<Actor> _toDelete = [];

    public Map(int width, int height)
    {
        Width = width;
        Height = height;

        solids = new bool[width * height];

        tiles = new Tile[width * height];
        // create tile objects
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tiles[x + Width*y] = new Tile( false, new ColoredGlyph() );
            }
        }

        // put solids to test
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x % 3 == 0 && y % 3 == 0)
                    tiles[x + Width*y] = new Tile( true, new ColoredGlyph(Color.Transparent, Color.AnsiRed) );
            }
        }

        // update solids
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                UpdateSolid(x, y);
            }
        }
    }

    public void Draw(ICellSurface surface)
    {
        // draw map tiles
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                surface.SetCellAppearance(x, y, GetTileAt(x, y).Appearance);
            }
        }
    }

    public void UpdateSolid(Point point)
    {
        if (!IsInBounds(point))
            return;

        // check for solid tiles
        if (tiles[point.X + point.Y*Width].Solid)
        {
            solids[point.X + point.Y*Width] = true;
            return;
        }

        // check for solid actors
        foreach(Actor actor in Actors)
        {
            if (actor.Solid && actor.Position == point)
            {
                solids[point.X + point.Y*Width] = true;
                return;
            }
        }

        // no solid points on this tile
        solids[point.X + point.Y*Width] = false;
    }

    public void UpdateSolid(int x, int y)
    {
        UpdateSolid((x, y));
    }

    public void AddActor(Actor actor)
    {
        Actors.Add(actor);
        UpdateSolid(actor.Position);
    }

    public void RemoveActor(Actor actor)
    {
        _toDelete.Add(actor);
        actor.QueuedForDeletion = true;
    }

    public void Flush()
    {
        foreach(Actor actor in _toDelete)
        {
            Actors.Remove(actor);
            actor.QueuedForDeletion = false; // disable being queued for deletion to allow item to act normally if brought back to a map
        }
        
        _toDelete.Clear();
    }

    public Tile GetTileAt(int x, int y)
    {
        return tiles[x + Width*y];
    }

    public Tile GetTileAt(Point point)
    {
        return GetTileAt(point.X, point.Y);
    }

    public bool IsInBounds(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return false;
        return true;
    }

    public bool IsInBounds(Point point)
    {
        return IsInBounds(point.X, point.Y);
    }

    public bool IsSolidAt(int x, int y)
    {
        return solids[x + y*Width];
    }

    public bool IsSolidAt(Point position)
    {
        return IsSolidAt(position.X, position.Y);
    }
}

public class Tile(bool solid, ColoredGlyph appearance)
{
    public bool Solid = solid;
    public ColoredGlyph Appearance = appearance;
}