namespace neon
{
    /// <summary>
    /// Event triggering when an entity is beeing moved from one archetype to another or when its active state changes.
    /// This is required to notify the cached Queries to rebuild.  
    /// </summary>
    public class ComponentStorageNotifier : EventReference<ComponentID> { }
}
