using System.Net.Http.Headers;
using System.Windows.Markup;

public class Effect : Persistant
{
    protected enum EffectType
    {
        BLANK,
        HEAL_ALL_LIMBS,
        HEAL_TARGET_LIMB
    }
    public virtual void Apply(Entity target, Entity owner, UiWindow? opener = null) {}
    public virtual void InverseApply() {}
    public override void Save(BinaryWriter writer) {writer.Write((int)EffectType.BLANK);}
    public override void Load(BinaryReader reader) {}

    public static Effect Create(BinaryReader reader)
    {
        EffectType type = (EffectType)reader.ReadInt32();
        Effect effect;
        
        switch (type)
        {
            case EffectType.HEAL_ALL_LIMBS: effect = new HealAllLimbsEffect(); break;
            case EffectType.HEAL_TARGET_LIMB: effect = new HealTargetLimbEffect(); break;
            default: effect = new(); break;
        }

        effect.Load(reader);
        return effect;
    }
}

public class HealAllLimbsEffect : Effect // heals all limbs random amount
{
    public int MinAmount = 1; // minimum amount of healing
    public int MaxAmount = 1; // maximum amount of healing

    public override void Apply(Entity target, Entity owner, UiWindow? opener = null)
    {
        if (target.Body == null)
            return;

        int healAmount = Engine.Rng.Next(MinAmount, MaxAmount);
        List<LimbData> limbs = [];
        BodyComponent.GetAllLimbs(limbs, target.Body.RootLimb);

        foreach (LimbData limb in limbs)
        {
            HealthManager.HealLimb(target, target.Body, limb, healAmount);
        }
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write((int)EffectType.HEAL_ALL_LIMBS);
        writer.Write(MinAmount);
        writer.Write(MaxAmount);
    }
    public override void Load(BinaryReader reader)
    {
        MinAmount = reader.ReadInt32();
        MaxAmount = reader.ReadInt32();
    }
}

public class HealTargetLimbEffect : Effect // heals a target limb and random amount
{
    public int MinAmount = 1; // minimum amount of healing
    public int MaxAmount = 1; // maximum amount of healing

    public override void Apply(Entity target, Entity owner, UiWindow? opener = null)
    {
        if (target.Body == null)
            return;
        
        int healAmount = Engine.Rng.Next(MinAmount, MaxAmount);
        LimbTargetWindow targetWindow = LimbTargetWindow.Create(target.Body, limb => { HealthManager.HealLimb(target, target.Body, limb, healAmount); }, "Select a limb to heal", false, false );
    }

    public override void Save(BinaryWriter writer)
    {
        writer.Write((int)EffectType.HEAL_TARGET_LIMB);
        writer.Write(MinAmount);
        writer.Write(MaxAmount);
    }
    public override void Load(BinaryReader reader)
    {
        MinAmount = reader.ReadInt32();
        MaxAmount = reader.ReadInt32();
    }
}