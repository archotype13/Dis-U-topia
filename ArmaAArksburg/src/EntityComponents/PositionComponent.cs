public class PositionComponent(int x, int y)
{
    public Point Cords {get; set;} = (x, y);
    public bool Solid {get; set;} = false;

    public bool Move(int x, int y) // returns whether the movement was solid or not. Checks for collision. To avoid collision and stuff, just set the cords
    {
        // return early if there is no level or point is out of bounds
        if ( Engine.Instance!.GameManager.CurrentLevel == null || Engine.Instance.GameManager.CurrentLevel.IsInBounds(x, y) == false)
            return false;

        // check if tile is solid
        if (Engine.Instance.GameManager.CurrentLevel.IsSolidAt(x, y))
            return false;
        
        // alter solids
        if (Solid)
        {
            Engine.Instance.GameManager.CurrentLevel.Grid.SetCellSolid(Cords, false);
            Engine.Instance.GameManager.CurrentLevel.Grid.SetCellSolid(x, y, true);
        }
        
        Cords = (x, y);
        return true;
    }

    public bool Move(Point point) {return Move(point.X, point.Y);}
}