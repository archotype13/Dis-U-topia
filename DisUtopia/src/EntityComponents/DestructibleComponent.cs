public sealed class DestructibleComponent : EntityComponent
{
    public int MaxHp {get; set;} = 1;
    public int Hp {get; set;} = 1;
    public int Av {get; set;} = 0;
    public int Dv {get; set;} = 0;
    public bool RequiresForced {get; set;} = false; // determines if the forced bool must be set to determine if there is a valid target. Useful for allowing doors and random objects to be destructible but also can be walked through without attacking
    public CorpseData? Corpse {get; set;} = null;

    public override void Save(BinaryWriter writer)
    {
        writer.Write(MaxHp);
        writer.Write(Hp);
        writer.Write(Av);
        writer.Write(Dv);
        writer.Write(RequiresForced);
    }

    public override void Load(BinaryReader reader)
    {
        MaxHp = reader.ReadInt32();
        Hp = reader.ReadInt32();
        Av = reader.ReadInt32();
        Dv = reader.ReadInt32();
        RequiresForced = reader.ReadBoolean();
    }
}