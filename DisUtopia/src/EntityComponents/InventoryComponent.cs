public class InventoryComponent : EntityComponent
{
    public int MaxWeight = 100;
    public int CurrentWeight = 0;
    public List<Entity> Items = [];

    public override void Save(BinaryWriter writer)
    {
        writer.Write(MaxWeight);
        writer.Write(CurrentWeight);

        writer.Write(Items.Count);
        foreach (Entity item in Items)
        {
            item.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        MaxWeight = reader.ReadInt32();
        CurrentWeight = reader.ReadInt32();

        int nItems = reader.ReadInt32();
        for (int i = 0; i < nItems; i++)
        {
            Entity item = new();
            item.Load(reader);
            Items.Add(item);
        }
    }
}