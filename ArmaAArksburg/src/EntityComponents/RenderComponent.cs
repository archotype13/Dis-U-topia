public sealed class RenderComponent(ColoredGlyph appearance) : EntityComponent
{
    public ColoredGlyph Appearance = appearance;
    public void Draw(Point position, ICellSurface surface)
    {
        if (Appearance.Foreground != Color.Transparent)
            surface[position].Foreground = Appearance.Foreground;
        if (Appearance.Background != Color.Transparent)
            surface[position].Background = Appearance.Background;
        if (Appearance.Glyph != ' ')
            surface[position].Glyph = Appearance.Glyph;
        surface[position].Mirror = Appearance.Mirror;
    }

    public override void Save(BinaryWriter writer)
    {
        SaveManager.SaveColoredGlyph(Appearance, writer);
    }

    public override void Load(BinaryReader reader)
    {
        Appearance = SaveManager.LoadColoredGlyph(reader);
    }
}