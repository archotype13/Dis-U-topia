using System.Net;

public sealed class BodyComponent : EntityComponent
{
    private const int REGEN_PERCENT = 5;
    private const int REGEN_DIVISOR = 100 / REGEN_PERCENT;

    public LimbData RootLimb {get; set;} = new(); // while set to be nullable
    public CorpseData? Corpse {get; set;} = null;
    public int DvMod {get; set;} = 0;
    public int TimeTillRegen {get; set;} = -1; // the amount of rounds of not taking damage until the creature begins to regen. -1 = never regen
    public int TicksSinceDamaged {get; set;} = 0; // the amount of ticks since the body last took damage
    public bool RequiresForced {get; set;} = false; // determines if the forced flag must be true to be targeted
    public bool IsAlive {get; set;} = true;

    public static void GetAllLimbs(List<LimbData> limbs, LimbData root) // recursively gets all the limbs of a selected limb. This goes into the limb list provided
    {
        limbs.Add(root);
        foreach(LimbData limb in root.ChildLimbs)
            GetAllLimbs(limbs, limb);
    }

    public override void Tick(Entity owner, Level level)
    {
        // handle regening
        TicksSinceDamaged++;
        if (TimeTillRegen != -1 && TicksSinceDamaged > TimeTillRegen)
        {
            List<LimbData> limbs = [];
            GetAllLimbs(limbs, RootLimb);
            foreach (LimbData limb in limbs)
            {
                HealthManager.HealLimb(owner, this, limb, Math.Max(limb.MaxHp / REGEN_DIVISOR, 1)); // regen by 5% of health of the limb or 1
            }
        }
    }

    // public override void AddToLevel(Entity owner, Level level)
    // {
    //     List<LimbData> limbs = [];
    //     GetAllLimbs(limbs, RootLimb);
    //     System.Console.WriteLine($"{owner.Name}'s body\nLimb count: {limbs.Count}");
    //     DebugPrintStatus();
    //     System.Console.Write('\n');
    // }

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
        // corpse data
        writer.Write(Corpse != null);
        Corpse?.Save(writer);

        writer.Write(DvMod);
        writer.Write(TimeTillRegen);
        writer.Write(TicksSinceDamaged);
        writer.Write(RequiresForced);
    }

    public override void Load(BinaryReader reader)
    {
        RootLimb.Load(reader);

        if (reader.ReadBoolean())
        {
            Corpse = new();
            Corpse.Load(reader);
        }

        DvMod = reader.ReadInt32();
        TimeTillRegen = reader.ReadInt32();
        TicksSinceDamaged = reader.ReadInt32();
        RequiresForced = reader.ReadBoolean();
    }
}