namespace neon
{
    /// <summary>
    /// Provides global methods to Add, Remove and Get <c>components</c>
    /// </summary>
    public static partial class Components
    {
        private static IComponentStorage storage;

        /// <summary>
        /// Set underlying component storage implementation
        /// </summary>
        public static void SetStorage(IComponentStorage storage)
        {
            Components.storage = storage;
        }

        public static bool Has<T>(EntityID entityID) where T : Component
        {
            return storage.Has<T>(entityID);
        }

        public static T Get<T>(EntityID entityID) where T : Component
        {
            return storage.Get<T>(entityID);
        }

        /// <summary>
        /// Get all <c>Components</c> attached to an entity
        /// </summary>
		public static Component[] GetAll(EntityID entityID) => storage.GetAll(entityID);

        public static (T1, T2) Get<T1, T2>(EntityID entityID) where T1 : Component where T2 : Component
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>()
            });

            return ((T1)rawComponents[0], ((T2)rawComponents[1]));
        }

        public static (T1, T2, T3) Get<T1, T2, T3>(EntityID entityID) where T1 : Component where T2 : Component where T3 : Component
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>(),
                Components.GetID<T3>()
            });

            return ((T1)rawComponents[0], (T2)rawComponents[1], (T3)rawComponents[2]);
        }

        public static (T1, T2, T3, T4) Get<T1, T2, T3, T4>(EntityID entityID) where T1 : Component where T2 : Component where T3 : Component where T4 : Component
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>(),
                Components.GetID<T3>(),
                Components.GetID<T4>()
            });

            return ((T1)rawComponents[0], (T2)rawComponents[1], (T3)rawComponents[2], (T4)rawComponents[3]);
        }

        public static bool TryGet<T>(EntityID entityID, out T result) where T : Component
        {
            result = storage.Get<T>(entityID);
            return result != null;
        }

        public static T Add<T>(EntityID entityID) where T : Component, new()
        {
            return storage.Add(entityID, new T());
        }

        public static T Add<T>(EntityID entityID, T inputComponent) where T : Component
        {
            return storage.Add(entityID, (T)inputComponent.Clone());
        }

        public static Component Add(EntityID entityID, Component inputComponent, Type type)
        {
            return storage.Add(entityID, inputComponent, type);
        }

        public static void Remove<T>(EntityID entityID) where T : Component
        {
            storage.Remove<T>(entityID);
        }

        public static void RemoveAll(EntityID entityID)
        {
            storage.Remove(entityID);
        }

        /// <summary>
        /// Get the ID of the entity the provided <c>Component</c> is attached to
        /// </summary>
        public static EntityID GetOwner<T>(T component) where T : Component => storage.GetOwner(component);

        public static T[] GetInChildren<T>(EntityID entityID, bool propagate = false) where T : Component
        {
            return storage.GetInChildren<T>(entityID, propagate);
        }

        public static T[] GetInParents<T>(EntityID entityID) where T : Component
        {
            return storage.GetInParents<T>(entityID);
        }
    }
}
