public sealed class DoorComponent : EntityComponent
{
    public ColoredGlyph OpenAppearance {get; set;} = new();
    public ColoredGlyph ClosedAppearance {get; set;} = new();
    public bool IsOpened {get; private set;} = false; // flag for whether the door is open or not. If you want it to be open by default, use the ToggleState function
    public int OpenCost = 100; // amount of AP required to toggle the state

    public bool ToggleState(Entity owner, bool open) // opens or closes the door
    {
        if (IsOpened == open) // return false if the door is already at that state
            return false;

        IsOpened = open;

        if (owner.Position != null)
        {
            owner.Position.Solid = !IsOpened;
            Engine.Instance!.GameManager.CurrentLevel!.Grid!.SetCellSolid(owner.Position.Cords, !IsOpened);
            Engine.Instance!.GameManager.CurrentLevel!.Grid.SetCellSolidThreshold(owner.Position.Cords, IsOpened? -1 : 1);
        }
        if (owner.Render != null)
        {
            owner.Render.Appearance = IsOpened? OpenAppearance : ClosedAppearance;
        }

        return true;
    }

    public override void AddToLevel(Entity owner, Level level)
    {
        if (owner.Render != null)
            owner.Render.Appearance = IsOpened? OpenAppearance : ClosedAppearance;
        if (owner.Position != null)
            level.Grid!.SetCellSolidThreshold(owner.Position.Cords, 1);
    }

    public override void Save(BinaryWriter writer)
    {
        SaveManager.SaveColoredGlyph(OpenAppearance, writer);
        SaveManager.SaveColoredGlyph(ClosedAppearance, writer);
        writer.Write(IsOpened);
        writer.Write(OpenCost);
    }

    public override void Load(BinaryReader reader)
    {
        OpenAppearance = SaveManager.LoadColoredGlyph(reader);
        ClosedAppearance = SaveManager.LoadColoredGlyph(reader);
        IsOpened = reader.ReadBoolean();
        OpenCost = reader.ReadInt32();
    }
}