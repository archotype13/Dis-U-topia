public sealed class AttackData : Persistant
{
    public int MinDamage {get; set;} = 1; // min damage in range
    public int MaxDamage {get; set;} = 1; // max damage in range
    public int Ap {get; set;} = 0;        // armor pierce bonus
    public int ToHit {get; set;} = 0;     // to hit bonus

    public override void Save(BinaryWriter writer)
    {
        writer.Write(MinDamage);
        writer.Write(MaxDamage);
        writer.Write(Ap);
        writer.Write(ToHit);
    }

    public override void Load(BinaryReader reader)
    {
        MinDamage = reader.ReadInt32();
        MaxDamage = reader.ReadInt32();
        Ap = reader.ReadInt32();
        ToHit = reader.ReadInt32();
    }
}