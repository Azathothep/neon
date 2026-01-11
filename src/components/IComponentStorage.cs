using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Interface container for any <c>Component</c> added to an entity.
    /// </summary>
    public interface IComponentStorage
    {
        IComponentIteratorProvider IteratorProvider { get; }

        /// <summary>
        /// Get a specific <c>Component</c> attached to the provided entity
        /// </summary>
        public T Get<T>(EntityID entityID) where T : Component;

        /// <summary>
        /// Get all <c>Components</c> attached to the provided entity
        /// </summary>
		public Component[] GetAll(EntityID entityID);

        public object[] GetComponentsFromIDs(EntityID entityID, ComponentID[] componentIDs);

        /// <summary>
        /// Request if the provided entity has a specific <c>Component</c>
        /// </summary>
        public bool Has<T>(EntityID entityID) where T : Component;

        /// <summary>
        /// Add a clone of the provided <c>Component</c> to the provided entity
        /// </summary>
        public T Add<T>(EntityID entityID, T component) where T : Component;

        /// <summary>
        /// Add a clone of the provided <c>Component</c> to the provided entity
        /// </summary>
        public Component Add(EntityID entityID, Component component, Type type);

        /// <summary>
        /// Remove the specified <c>Component</c> from the provided entity
        /// </summary>
        public void Remove<T>(EntityID entityID) where T : Component;

        /// <summary>
        /// Remove the specified <c>Component</c> from the provided entity
        /// </summary>
        public void Remove(EntityID entityID);

        /// <summary>
        /// Get the entity the provided component is attached to
        /// </summary>
        public EntityID GetOwner<T>(T component) where T : Component;

        public T[] GetInChildren<T>(EntityID entityID, bool propagate = false) where T : Component;

        public T[] GetInParents<T>(EntityID entityID) where T : Component;
    }
}
