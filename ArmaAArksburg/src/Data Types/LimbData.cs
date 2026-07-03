public sealed class LimbData : Persistant
{
    public string Name {get; set;} = "unknown limb";
    public int MaxHp {get; set;} = 1;
    public int Hp {get; set;} = 1;
    public int Av {get; set;} = 0;
    public int Dv {get; set;} = 0;
    public bool Vital {get; set;} = false; // determines if the limb is needed to live
    public List<LimbData> ChildLimbs = [];

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(MaxHp);
        writer.Write(Hp);
        writer.Write(Av);
        writer.Write(Dv);
        writer.Write(Vital);

        writer.Write(ChildLimbs.Count);
        foreach (LimbData limb in ChildLimbs)
        {
            limb.Save(writer);
        }
    }

    public override void Load(BinaryReader reader)
    {
        Name = reader.ReadString();
        MaxHp = reader.ReadInt32();
        Hp = reader.ReadInt32();
        Av = reader.ReadInt32();
        Dv = reader.ReadInt32();
        Vital = reader.ReadBoolean();

        int nChilds = reader.ReadInt32();
        for (int i = 0; i < nChilds; i++)
        {
            LimbData limb = new();
            limb.Load(reader);
            ChildLimbs.Add(limb);
        }
    }
}