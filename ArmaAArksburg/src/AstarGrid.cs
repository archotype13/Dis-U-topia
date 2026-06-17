using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;

public class AStarGrid // used to track solid and weights for pathfinding and collision detection!
{
    public int Width;
    public int Height;
    private ValueTuple<int, int>[] _cells; // first int is the amount of solids, second is the amount of weight

    public bool IsInBounds(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            return true;
        return false;
    }

    public bool IsInBounds(Point point) { return IsInBounds(point.X, point.Y); }

    // get cell data
    public ValueTuple<int, int> GetCell(int x, int y)
    {
        return _cells[x + y*Width];
    }

    public ValueTuple<int, int> GetCell(Point point) {return GetCell(point.X, point.Y);}

    // change a cells solid amount
    public void SetCellSolid(int x, int y, bool solid)
    {
        _cells[x + y*Width].Item1 = Math.Max(_cells[x + y*Width].Item1 + (solid? 1 : -1), 0);
    }

    public void SetCellSolid (Point point, bool solid) {SetCellSolid(point.X, point.Y, solid);}

    // set the movement cost. AddOrRemove determines whether or not to add or remove cost
    public void SetCellMoveCost(int x, int y, bool addOrRemove, int amount)
    {
        _cells[x + y*Width].Item2 = Math.Max( _cells[x + y*Width].Item2 + (addOrRemove? amount : -amount), 0);
    }

    public void SetCellMoveCost(Point point, bool addOrRemove, int amount) {SetCellMoveCost(point.X, point.Y, addOrRemove, amount);}

    public AStarGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new ValueTuple<int, int>[width * height];
    }
}