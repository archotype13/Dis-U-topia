using System.Formats.Asn1;

public sealed class BodyComponent : EntityComponent
{
    public LimbData RootLimb {get; set;} = new(); // while set to be nullable
    public CorpseData? Corpse {get; set;} = null;
    public int DvMod {get; set;} = 0;
    public bool RequiresForced {get; set;} = false; // determines if the forced flag must be true to be targeted
    public bool IsAlive {get; set;} = true;

    public static void GetAllLimbs(List<LimbData> limbs, LimbData root) // recursively gets all the limbs of a selected limb. This goes into the limb list provided
    {
        limbs.Add(root);
        foreach(LimbData limb in root.ChildLimbs)
            GetAllLimbs(limbs, limb);
    }

    public override void AddToLevel(Entity owner, Level level)
    {
        // List<LimbData> limbs = [];
        // GetAllLimbs(limbs, RootLimb);
        // System.Console.WriteLine($"{owner.Name}'s body\nLimb count: {limbs.Count}");
        // DebugPrintStatus();
        // System.Console.Write('\n');
    }

    public void DebugPrintStatus() // begins to print the data of all of the limbs
    {
        DebugLimbStatus(RootLimb, 0);
        System.Console.Write('\n');
    }

    private void DebugLimbStatus(LimbData limb, int generation) // helper function to resursively print out limb data
    {
        string tabs = new('\t', generation);
        System.Console.WriteLine($"{tabs}{limb.Name}: ({limb.Hp} / {limb.MaxHp})\n{tabs}Vital: {limb.Vital}\n{tabs}Children:");

        foreach (LimbData child in limb.ChildLimbs) // print out the children limb data
        {
            DebugLimbStatus(child, generation + 1);
        }
        System.Console.Write('\n');
    }

    public override void Save(BinaryWriter writer)
    {
        RootLimb.Save(writer);
        writer.Write(DvMod);
        writer.Write(RequiresForced);
    }

    public override void Load(BinaryReader reader)
    {
        RootLimb.Load(reader);
        DvMod = reader.ReadInt32();
        RequiresForced = reader.ReadBoolean();
    }
}