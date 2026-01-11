namespace neon
{
    /// <summary>
    /// Events relative to a <c>Component</c>
    /// </summary>
    public enum ComponentHook
    {
        /// <summary>
        /// Called when a component is added to an entity
        /// </summary>
        OnAdded,

        /// <summary>
        /// Called when a component is removed from an entity
        /// </summary>
        OnRemoved
    }
}
