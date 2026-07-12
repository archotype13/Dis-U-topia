public abstract class EntityComponent : Persistant
{
    public virtual void Tick(Entity owner, Level level)
    {
        
    }
    public virtual void AddToLevel(Entity owner, Level level)
    {
        
    }

    public virtual void RemoveFromLevel(Entity owner, Level level)
    {
        
    }
}