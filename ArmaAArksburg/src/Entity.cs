public sealed class Entity : Persistant
{
    public string Name {get; set;} = "Unnamed";
    public PositionComponent? Position {get; set;}
    public RenderComponent? Render {get; set;}
    public DestructibleComponent? Destructible {get; set;}
    public BodyComponent? Body {get; set;}
    public AttackComponent? Attack {get; set;}
    public AiComponent? Ai {get; set;}
    public DoorComponent? Door {get; set;}

    public void AddToLevel(Level level)
    {
        Position?.AddToLevel(this, level);
        Render?.AddToLevel(this, level);
        Destructible?.AddToLevel(this, level);
        Body?.AddToLevel(this, level);
        Attack?.AddToLevel(this, level);
        Door?.AddToLevel(this, level);
        Ai?.AddToLevel(this, level);
    }

    public void RemoveFromLevel(Level level)
    {
        Position?.RemoveFromLevel(this, level);
        Render?.RemoveFromLevel(this, level);
        Destructible?.RemoveFromLevel(this, level);
        Body?.RemoveFromLevel(this, level);
        Attack?.AddToLevel(this, level);
        Door?.RemoveFromLevel(this, level);
        Ai?.RemoveFromLevel(this, level);
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(Position != null);      // Position
        Position?.Save(writer);
        writer.Write(Render != null);        // Rendering
        Render?.Save(writer);
        writer.Write(Destructible != null);  // Destructible
        Destructible?.Save(writer);
        writer.Write(Body != null);          // Body
        Body?.Save(writer);
        writer.Write(Attack != null);        // Attack
        Attack?.Save(writer);
        writer.Write(Ai != null);            // Ai
        Ai?.Save(writer);
        writer.Write(Door != null);          // Door
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
            Render = new(new(), 0);
            Render.Load(reader);
        }
        if (reader.ReadBoolean()) // check if destructible exists
        {
            Destructible = new();
            Destructible.Load(reader);
        }
        if (reader.ReadBoolean()) // check if body exists
        {
            Body = new();
            Body.Load(reader);
        }
        if (reader.ReadBoolean()) // check if attack exists
        {
            Attack = new();
            Attack.Load(reader);
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