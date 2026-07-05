public static class HealthManager // manages healing and damaging destructible and body components
{
    // balancing constants
    const int N_AV_ROLLS = 3;
    const int AV_ROLL_MAX = 20;
    const int DV_ROLL_MAX = 20;

    private struct AttackResult(int damage, int pierces, bool missed)
    {
        public int Damage = damage;
        public int Pierces = pierces;
        public bool Missed = missed;
    }

    public static bool IsAlive( Entity entity ) // determines if a destructible is alive or not
    {
        if (entity.Body != null)
            return entity.Body.IsAlive;
        if (entity.Destructible != null)
            return IsAlive(entity.Destructible);
        return true;
    }

    public static bool IsAlive( DestructibleComponent destructible ) // determines if a destructible is alive or not
    {
        return (destructible.Hp > 0)? true : false;
    }

    public static bool IsTargetable( Entity entity, bool forced ) // checks if an entity is a valid target
    {
        if ( entity.Destructible != null && (!entity.Destructible.RequiresForced || forced) ) // check for a destructible and if it needs forced or not
            return true;
        if ( entity.Body != null && (!entity.Body.RequiresForced || forced) ) // check for a body and if it needs forced or not
            return true;

        return false;
    }

    public static void DealDamage( Entity target, Entity attacker, AttackData attackData ) // determines what components to deal damage to
    {
        if (target.Body != null)
            DealDamage(target, target.Body, attacker, attackData);
        else if (target.Destructible != null)
            DealDamage(target, target.Destructible, attacker, attackData);
    }

    private static void DealDamage( Entity target, DestructibleComponent destructible, Entity attacker, AttackData attackData ) // deals damage to destructibles
    {
        AttackResult result = GetAttackResult(attackData, destructible!.Av, destructible!.Dv);

        if (result.Missed)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}[c:u] misses");
            return;
        }

        if (result.Pierces == 0)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}'s[c:u] attack fails to pierce the armor of [c:r f:Yellow]{target.Name}[c:u]");
            return;
        }

        target.Destructible!.Hp -= result.Damage;
        Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}[c:u] hits [c:r f:Yellow]{target.Name}[c:u] for [c:r f:Red]{result.Damage} ({result.Pierces})[c:u] points of damage ({target.Destructible.Hp}/{target.Destructible.MaxHp})");

        if ( !IsAlive(destructible) )
            KillEntity(target, destructible.Corpse);
    }

    private static void DealDamage( Entity target, BodyComponent body, Entity attacker, AttackData attackData ) // deals damage to bodies. Currently targets a random limb
    {
        List<LimbData> limbs = [];
        BodyComponent.GetAllLimbs(limbs, body.RootLimb);
        List<LimbData> aliveLimbs = [.. limbs.Where(limb => limb.Hp > 0)];
        int index = Engine.Rng.Next(0, aliveLimbs.Count);
        DealDamage(target, body, aliveLimbs[index], attacker, attackData);
    }

    private static void DealDamage(Entity target, BodyComponent body, LimbData limb, Entity attacker, AttackData attackData) // deals damage to a certain limb
    {
        AttackResult result = GetAttackResult(attackData, limb.Av, limb.Dv + body.DvMod);

        if (result.Missed)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}[c:u] misses");
            return;
        }

        if (result.Pierces == 0)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}'s[c:u] attack fails to pierce the armor of [c:r f:Yellow]{target.Name}'s {limb.Name}[c:u]");
            return;
        }

        limb.Hp -= result.Damage;
        Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}[c:u] hits [c:r f:Yellow]{target.Name}'s {limb.Name}[c:u] for [c:r f:Red]{result.Damage} ({result.Pierces})[c:u] points of damage ({limb.Hp}/{limb.MaxHp})");

        // handle death
        if ( limb.Vital && limb.Hp <= 0 )
        {
            body.IsAlive = false;
            KillEntity(target, body.Corpse);
        }
            
    }

    private static AttackResult GetAttackResult(AttackData attack, int av, int dv) // determines the amount of damage something takes
    {
        // dv roll
        if (Engine.Rng.Next(0, DV_ROLL_MAX) + attack.ToHit < dv)
        {
            return new(0, 0, true);
        }

        int damage = 0;
    
        // av rolls
        int pierces = 0;
        for (int i = 0; i < N_AV_ROLLS; i++)
        {
            int roll = Engine.Rng.Next(0, AV_ROLL_MAX + 1);
            if (roll + attack.Ap >= av || roll == AV_ROLL_MAX) // only hit if the attack pierces armor or crits
            {
                damage += Engine.Rng.Next(attack.MinDamage, attack.MaxDamage + 1);
                pierces++;
            }
                
        }

        return new(damage, pierces, false);
    }

    private static void KillEntity( Entity target, CorpseData? corpseData ) // manages entity death for destructibles
    {
        Engine.Instance!.GameManager.CurrentLevel!.RemoveEntity(target);

        // corpse data
        if (corpseData != null && target.Position != null)
        {
            Entity corpse = new()
            {
                Name = corpseData.CorpseName,
                Position = new(target.Position.Cords.X, target.Position.Cords.Y) {Solid = false},
                Render = new(corpseData.Appearance, 0)
            };

            Engine.Instance!.GameManager.CurrentLevel!.AddEntity(corpse);
        }
    }
}