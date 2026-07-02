public sealed class CorpseData : Persistant // determines what kind of an attackable entity should leave behind
{
    public string CorpseName = "unknown corpse";
    public ColoredGlyph Appearance {get; set;} = new();

    public override void Save(BinaryWriter writer)
    {
        writer.Write(CorpseName);
        SaveManager.SaveColoredGlyph(Appearance, writer);
    }

    public override void Load(BinaryReader reader)
    {
        CorpseName = reader.ReadString();
        Appearance = SaveManager.LoadColoredGlyph(reader);
    }
}