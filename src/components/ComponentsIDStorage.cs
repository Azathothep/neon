using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace neon
{
    public partial class Components
    {
        private class ComponentsIDStorage {
            public Dictionary<Type, ComponentID> ComponentToID = new();
            public Dictionary<ComponentID, Type> IDToComponent = new();
            public int LastID = -1;
        }

        private static ComponentsIDStorage idStorage = new();

        /// <summary>
        /// Get the associated <c>Type</c> of a Component from its <c>ComponentID</c>
        /// </summary>
        public static Type GetType(ComponentID component)
        {
            return idStorage.IDToComponent[component];
        }

        public static ComponentID GetID<T>() where T : Component
        {
            Type componentType = typeof(T);

            return GetIDByTypeUnsafe(componentType);
        }

        public static ComponentID GetIDByType(Type componentType)
        {
            if (componentType.IsAssignableFrom(typeof(Component)))
                return null;

            return GetIDByTypeUnsafe(componentType);
        }

        /// <summary>
        /// Same as <c>GetIDByType</c> without verification if the provided type derives from <c>Component</c>.
        /// </summary>
        private static ComponentID GetIDByTypeUnsafe(Type componentType)
        {
            if (idStorage.ComponentToID.TryGetValue(componentType, out ComponentID componentID))
                return componentID;

            componentID = new ComponentID(++idStorage.LastID);

            AddUnsafe(componentType, componentID);

            return componentID;
        }

        private static void AddUnsafe(Type componentType, ComponentID id)
        {
            idStorage.ComponentToID.Add(componentType, id);
            idStorage.IDToComponent.Add(id, componentType);
        }
    }
}
