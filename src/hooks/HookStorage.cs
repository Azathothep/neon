using System.Diagnostics;
using System.Net;

namespace neon
{
    /// <summary>
    /// Implements <c>IHookStorage</c>. Stores and interacts with <c>HookMaps</c>.
    /// Hooks can be global-scoped or object-scoped. They are always linked to a target object (entity, component...).
    /// A global-scoped hook is called when the hook is raised, independently of the target object.
    /// A object-scoped hook is called on a specific object when the hook is raised on it. 
    /// </summary>
    public class HookStorage : IHookStorage
    {
        private Dictionary<IHookType, IHookMap> m_Hooks = new();

        public HookStorage() { }

        /// <summary>
        /// Creates a new <c>HookMap</c> with the specified <c>HookID</c> and returns its corresponding <c>HookTrigger</c>.
        /// You can provide an additional type to restraint the hook scope (useful for <c>Component</c> hooks).
        /// </summary>
        public HookTrigger<HookID> Create<HookID>(Type additionalType = null) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>(additionalType);

            if (m_Hooks.ContainsKey(type))
                return null;

            IHookMap map = new HookMap<HookID>();
            m_Hooks.Add(type, map);

            HookTrigger<HookID> trigger = new HookTrigger<HookID>((h, o) => Trigger(h, o));

            return trigger;
        }

        /// <summary>
        /// Creates a new <c>HookMap</c> with the specified <c>HookID</c> and returns its corresponding <c>HookTrigger</c>.
        /// </summary>
        public HookTrigger<HookID> Create<HookID, T>() where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (m_Hooks.ContainsKey(type))
                return null;

            IHookMap map = new HookMap<HookID>();
            m_Hooks.Add(type, map);

            HookTrigger<HookID> trigger = new HookTrigger<HookID>((h, o) => Trigger<HookID, T>(h, o));

            return trigger;
        }

        /// <summary>
        /// Raise all global-scope events subscribed to the specified HookID, and all object-scoped event targetting the provided object
        /// </summary>
        private void Trigger<HookID>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        /// <summary>
        /// Raise all global-scope events subscribed to the specified type-restrained HookID, and all object-scoped event targetting the provided object
        /// </summary>
        private void Trigger<HookID, T>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        /// <summary>
        /// Subscribe to the specified HookID in object-scope
        /// </summary>
        public void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        /// <summary>
        /// Subscribe to the specified HookID in global-scope
        /// </summary>
        public void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action);
        }

        /// <summary>
        /// Subscribe to the specified type-restrained HookID in object-scope
        /// </summary>
        public void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)}, {typeof(T)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        /// <summary>
        /// Subscribe to the specified type-restrained HookID in global-scope
        /// </summary>
        public void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)}, {typeof(T)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action);
        }

        /// <summary>
        /// Unsubscribe to the specified HookID targetting provided object
        /// </summary>
        public void Remove<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action, o);
        }

        /// <summary>
        /// Unsubscribe to the specified HookID in global-scope
        /// </summary>
        public void Remove<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action);
        }

        /// <summary>
        /// Unsubscribe to the specified type-restrained HookID targetting provided object
        /// </summary>
        public void Remove<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action, o);
        }

        /// <summary>
        /// Unsubscribe to the specified type-restrained HookID in global-scope
        /// </summary>
        public void Remove<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action);
        }
    }
}
