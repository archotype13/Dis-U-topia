public sealed class RenderComponent(ColoredGlyph appearance, int priority) : EntityComponent
{
    private EntityRenderable? _entityRenderable;
    public ColoredGlyph Appearance = appearance;
    public int Priority = priority;
    public bool DrawWhenExplored = false; // determines if it's drawn when explored and not visible

    public override void AddToLevel(Entity owner, Level level)
    {
        if (owner.Position == null)
            return;

        _entityRenderable = new(owner.Position, this, Priority);
        Engine.Instance!.ScreenManager.WorldView.AddRenderable(_entityRenderable);
    }

    public override void RemoveFromLevel(Entity owner, Level level)
    {
        if (_entityRenderable != null)
            Engine.Instance!.ScreenManager.WorldView.RemoveRenderable(_entityRenderable);
    }

    public override void Save(BinaryWriter writer)
    {
        SaveManager.SaveColoredGlyph(Appearance, writer);
        writer.Write(Priority);
        writer.Write(DrawWhenExplored);
    }

    public override void Load(BinaryReader reader)
    {
        Appearance = SaveManager.LoadColoredGlyph(reader);
        Priority = reader.ReadInt32();
        DrawWhenExplored = reader.ReadBoolean();
    }
}