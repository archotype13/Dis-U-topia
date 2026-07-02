public sealed class AttackComponent : EntityComponent // contains the melee attack information for melee attacking
{
    public AttackData Attack {get; set;} = new();

    public override void Save(BinaryWriter writer)
    {
        Attack.Save(writer);
    }

    public override void Load(BinaryReader reader)
    {
        Attack.Load(reader);
    }
}