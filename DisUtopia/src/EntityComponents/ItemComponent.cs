public sealed class ItemComponent : EntityComponent
{
    public int Weight = 0;

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Weight);
    }

    public override void Load(BinaryReader reader)
    {
        Weight = reader.ReadInt32();
    }
}