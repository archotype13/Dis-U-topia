public sealed class AStarGrid // used to track solid and weights for pathfinding and collision detection!
{
    public struct AstarTile()
    {
        public int NSolids = 0;
        public int MoveCost = 0;
    }

    public int Width;
    public int Height;
    private AstarTile[] _cells; // first int is the amount of solids, second is the amount of weight

    public bool IsInBounds(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            return true;
        return false;
    }

    public bool IsInBounds(Point point) { return IsInBounds(point.X, point.Y); }

    // get cell data
    public AstarTile GetCell(int x, int y)
    {
        return _cells[x + y*Width];
    }

    public AstarTile GetCell(Point point) {return GetCell(point.X, point.Y);}

    // change a cells solid amount
    public void SetCellSolid(int x, int y, bool solid)
    {
        _cells[x + y*Width].NSolids = Math.Max(_cells[x + y*Width].NSolids + (solid? 1 : -1), 0);
    }

    public void SetCellSolid (Point point, bool solid) {SetCellSolid(point.X, point.Y, solid);}

    // set the movement cost. AddOrRemove determines whether or not to add or remove cost
    public void SetCellMoveCost(int x, int y, bool addOrRemove, int amount)
    {
        _cells[x + y*Width].MoveCost = Math.Max( _cells[x + y*Width].MoveCost + (addOrRemove? amount : -amount), 0);
    }

    public void SetCellMoveCost(Point point, bool addOrRemove, int amount) {SetCellMoveCost(point.X, point.Y, addOrRemove, amount);}

    public AStarGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new AstarTile[width * height];
    }
}