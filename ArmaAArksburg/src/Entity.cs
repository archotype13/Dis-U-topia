public sealed class Entity
{
    public string Name {get; set;} = "Unnamed";
    public PositionComponent? Position {get; set;}
    public RenderComponent? Render {get; set;}
    public AiComponent? Ai {get; set;}
    public DoorComponent? Door {get; set;}

    // public virtual 
}