namespace neon
{
    /// <summary>
    /// Events relative to an entity
    /// </summary>
    public enum EntityHook
    {
        /// <summary>
        /// Called when the entity is activated
        /// </summary>
        OnEnabled,

        /// <summary>
        /// Called when the entity is deactivated
        /// </summary>
        OnDisabled,

        /// <summary>
        /// Called when the entity's parent changes
        /// </summary>
        OnNewParent,

        /// <summary>
        /// Called when the entity loses or has a new child 
        /// </summary>
        OnNewChild
    }
}
