namespace neon
{
    /// <summary>
    /// Base class for components. Every component must derive from this class.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Each component is also considered as an Entity in itself.
        /// This is the ID of the component.
        /// For getting the ID of the entity this component is attached to, get the <c>Owner</c> property.
        /// </summary>
        public EntityID EntityID => m_EntityID;
        private EntityID m_EntityID = Entities.GetID(true);

        /// <summary>
        /// Creates a deep copy of this component
        /// </summary>
        public abstract Component Clone();

        /// <summary>
        /// The ID of the entity this <c>Component</c> is attached to
        /// </summary>
        public EntityID Owner => Components.GetOwner(this);

        /// <summary>
        /// Add a new <c>Component</c> on the same entity this component is attached to
        /// </summary>
        public T Add<T>() where T : Component, new() => this.Owner.Add<T>();

        /// <summary>
        /// Add a new <c>Component</c> on the same entity this component is attached to
        /// </summary>
        public T Add<T>(T component) where T : Component => this.Owner.Add(component);

        /// <summary>
        /// Remove a <c>Component</c> in the same entity this component is attached to
        /// </summary>
        public void Remove<T>() where T : Component => this.Owner.Remove<T>();

        /// <summary>
        /// Get a <c>Component</c> on the same entity this component is attached to
        /// </summary>
        public T Get<T>() where T : Component => this.Owner.Get<T>();
    }
}
