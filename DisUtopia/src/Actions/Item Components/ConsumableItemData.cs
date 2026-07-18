public sealed class ConsumableItemData : ItemData
{
    public void Consume(Entity actor, Entity owner, UiWindow? opener = null)
    {
        actor.Inventory!.RemoveItem(owner);
    }

    public override void Save(BinaryWriter writer)
    {
        
    }

    public override void Load(BinaryReader reader)
    {
        
    }
}