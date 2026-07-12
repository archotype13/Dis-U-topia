using System.Formats.Asn1;

public sealed class PositionComponent(int x, int y) : EntityComponent
{
    public Point Cords {get; set;} = (x, y);
    public bool Solid {get; set;} = false;
    public bool Opaque {get; set;} = false;

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
            Engine.Instance.GameManager.CurrentLevel.Grid!.SetCellSolid(Cords, false);
            Engine.Instance.GameManager.CurrentLevel.Grid.SetCellSolid(x, y, true);
        }
        // alter opaque
        if (Opaque)
        {
            Engine.Instance.GameManager.CurrentLevel.Grid!.SetCellOpaque(Cords, false);
            Engine.Instance.GameManager.CurrentLevel.Grid.SetCellOpaque(x, y, true);
        }
        
        Cords = (x, y);
        return true;
    }

    public bool Move(Point point) {return Move(point.X, point.Y);}


    public override void AddToLevel(Entity owner, Level level)
    {
        if (owner.Door != null) // if the owner has a door component, just let the door handle solid and opaque-ness
            return;

        if (Solid)
            level.Grid!.SetCellSolid(Cords, true);
        if (Opaque)
            level.Grid!.SetCellOpaque(Cords, true);
    }

    public override void RemoveFromLevel(Entity owner, Level level)
    {
        if (Solid) // remove solid from map if solid
            level.Grid.SetCellSolid(Cords, false);
        if (Opaque)
            level.Grid!.SetCellOpaque(Cords, false);
    }

    public override void Save(BinaryWriter writer)
    {
        SaveManager.SavePoint(Cords, writer);
        writer.Write(Solid);
        writer.Write(Opaque);
    }

    public override void Load(BinaryReader reader)
    {
        Cords = SaveManager.LoadPoint(reader);
        Solid = reader.ReadBoolean();
        Opaque = reader.ReadBoolean();
    }

}