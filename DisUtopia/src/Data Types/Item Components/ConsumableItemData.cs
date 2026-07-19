public sealed class ConsumableItemData : ItemData
{
    public List<Effect> Effects = [];

    public void Consume(Entity actor, Entity owner, UiWindow? opener = null)
    {
        // print a message
        if (actor == Engine.Instance!.GameManager.Player)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"You consume the {owner.Name}");
        }

        foreach (Effect effect in Effects)
        {
            effect.Apply(actor, owner, opener);
        }

        actor.Inventory!.RemoveItem(owner);
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Effects.Count);
        foreach (Effect effect in Effects)
        {
            effect.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        int effectCount = reader.ReadInt32();
        for (int i = 0; i < effectCount; i++)
        {
            Effects.Add( Effect.Create(reader) );
        }
        
    }
}