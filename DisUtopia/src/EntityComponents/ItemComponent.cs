public sealed class ItemComponent : EntityComponent
{
    public int Weight = 0;
    public ConsumableItemData? Consumable;

    public void Consume()
    {
        
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Weight);

        // consumable data
        writer.Write(Consumable != null);
        Consumable?.Save(writer);
    }

    public override void Load(BinaryReader reader)
    {
        Weight = reader.ReadInt32();

        // consumable data
        if (reader.ReadBoolean())
        {
            Consumable = new();
            Consumable.Load(reader);
        }
    }
}