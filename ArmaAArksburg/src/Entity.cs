public sealed class Entity : Persistant
{
    public string Name {get; set;} = "Unnamed";
    public PositionComponent? Position {get; set;}
    public RenderComponent? Render {get; set;}
    public AiComponent? Ai {get; set;}
    public DoorComponent? Door {get; set;}

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(Position != null);
        Position?.Save(writer);
        writer.Write(Render != null);
        Render?.Save(writer);
        writer.Write(Ai != null);
        Ai?.Save(writer);
        writer.Write(Door != null);
        Door?.Save(writer);
    }

    public override void Load(BinaryReader reader)
    {
        Name = reader.ReadString();
        if (reader.ReadBoolean()) // check if position exists
        {
            Position = new(0, 0);
            Position.Load(reader);
        }
        if (reader.ReadBoolean()) // check if render exists
        {
            Render = new(new());
            Render.Load(reader);
        }
        if (reader.ReadBoolean()) // check if ai exists
        {
            Ai = AiComponent.Create(reader);
        }
        if (reader.ReadBoolean()) // check if door exists
        {
            Door = new();
            Door.Load(reader);
        }
    }
}