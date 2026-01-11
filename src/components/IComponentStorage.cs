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

        public T Get<T>(EntityID entityID) where T : Component;

		public Component[] GetAll(EntityID entityID);

        public object[] GetComponentsInternal(EntityID entityID, ComponentID[] componentIDs);

        public bool Has<T>(EntityID entityID) where T : Component;

        public T Add<T>(EntityID entityID, T component) where T : Component;

        public Component Add(EntityID entityID, Component component, Type type);

        public void Remove<T>(EntityID entityID) where T : Component;

        public void Remove(EntityID entityID);

        public EntityID GetOwner<T>(T component) where T : Component;

        public T[] GetInChildren<T>(EntityID entityID, bool propagate = false) where T : Component;

        public T[] GetInParents<T>(EntityID entityID) where T : Component;
    }
}
