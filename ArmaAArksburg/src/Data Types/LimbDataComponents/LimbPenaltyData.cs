public sealed class LimbPenaltyData : Persistant
{
    public float Weight; // amount of limbs
    public int Penalty; // amount of currently applied speed penalty

    public override void Save(BinaryWriter writer)
    {
        writer.Write(Weight);
        writer.Write(Penalty);
    }

    public override void Load(BinaryReader reader)
    {
        Weight = reader.ReadSingle();
        Penalty = reader.ReadInt32();
    }
}