public abstract class Persistant
{
    public abstract void Save(BinaryWriter writer);
    public abstract void Load(BinaryReader reader);
}