using System.Security.Cryptography.X509Certificates;
using Coroutine;
using SadRogue.Primitives.SpatialMaps;

public class InventoryComponent : EntityComponent
{
    public int MaxWeight = 100;
    public int CurrentWeight = 0;
    public List<Entity> Items = []; // while exposed for ui purposes, use AddItem and RemoveItem methods to alter contents

    public event EventHandler<ItemsChangedEventArgs>? OnChanged;
    public struct ItemsChangedEventArgs(Entity entity)
    {
        public Entity Entity = entity;
    }

    public bool AddItem(Entity entity, bool forced = false)
    {
        if (entity.Item == null)
            return false;
        
        if (entity.Item.Weight + CurrentWeight > MaxWeight && forced == false)
            return false;

        Items.Add(entity);
        CurrentWeight += entity.Item.Weight;
        
        OnChanged?.Invoke(this, new(entity));
        return true;
    }

    public void RemoveItem(Entity entity)
    {
        Items.Remove(entity);
        CurrentWeight -= entity.Item!.Weight;
        
        OnChanged?.Invoke(this, new(entity));
    }

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