public static class HealthManager // manages healing and damaging destructible and body components
{
    // balancing constants
    const int N_AV_ROLLS = 3;
    const int AV_ROLL_MAX = 20;
    const int DV_ROLL_MAX = 20;


    public static bool IsAlive( Entity entity ) // checks an entity for if they are dead or not. Returns true if it doesn't have a valid component to check
    {
        if ( entity.Destructible != null ) // if the entity has a destructible, use the correct overload
            return IsAlive(entity.Destructible);
        return true;
    }

    public static bool IsAlive( DestructibleComponent destructible ) // determines if a destructible is alive or not
    {
        return (destructible.Hp > 0)? true : false;
    }

    public static bool IsTargetable( Entity entity, bool forced ) // checks if an entity is a valid target
    {
        if ( entity.Destructible != null && (!entity.Destructible.RequiresForced || forced) ) // check for a ALIVE destructible and if it needs forced or not
            return true;

        return false;
    }

    public static void DealDamage( Entity target, Entity attacker, AttackData attackData ) // determines what components to deal damage to
    {
        if (target.Destructible != null)
            DealDamage<DestructibleComponent>(target, attacker, attackData);
    }

    private static void DealDamage<DestructibleComponent>( Entity target, Entity attacker, AttackData attackData ) // deals damage to destructibles
    {
        // dv roll
        if (Engine.Rng.Next(0, DV_ROLL_MAX) + attackData.ToHit < target.Destructible!.Dv)
        {
            Engine.Instance!.ScreenManager.Log.LogMessage($"{attacker.Name} misses!");
        }

        int damage = 0;
    
        // av rolls
        int pierces = 0;
        for (int i = 0; i < N_AV_ROLLS; i++)
        {
            int roll = Engine.Rng.Next(0, AV_ROLL_MAX + 1);
            if (roll + attackData.Ap >= target.Destructible!.Av || roll == AV_ROLL_MAX) // only hit if the attack pierces armor or crits
            {
                damage += Engine.Rng.Next(attackData.MinDamage, attackData.MaxDamage + 1);
                pierces++;
            }
                
        }

        target.Destructible!.Hp -= damage;
        Engine.Instance!.ScreenManager.Log.LogMessage($"[c:r f:Yellow]{attacker.Name}[c:u] hits [c:r f:Yellow]{target.Name}[c:u] for [c:r f:Red]{damage} ({pierces})[c:u] points of damage ({target.Destructible.Hp}/{target.Destructible.MaxHp})");

        if ( !IsAlive(target.Destructible) )
            KillEntity(target);
    }

    private static void KillEntity( Entity target ) // manages entity death
    {
        Engine.Instance!.GameManager.CurrentLevel!.RemoveEntity(target);
    }
}