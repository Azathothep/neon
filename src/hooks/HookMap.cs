namespace neon
{
    /// <summary>
    /// Stores all <c>Hooks</c> relative to specific HookID.
    /// </summary>
    public class HookMap<HookID> : IHookMap where HookID : struct, IConvertible
    {
        private Dictionary<HookID, (MainHook, Dictionary<object, Hook>)> m_Hooks = new();

        /// <summary>
        /// If at least one action has subscribed to it, raise the provided hook in global-scope and in the provided object's-scope
        /// </summary>
        public void Trigger(HookID hook, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook mainHook, Dictionary<object, Hook> objectHooks) = hooks;

            mainHook.Raise(o);

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
                return;

            objectHook.Raise();
        }

        /// <summary>
        /// Subscribe a new action to the provided object-scoped hook
        /// </summary>
        public void Add(HookID hook, Action action, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
            {
                hooks = (new MainHook(), new Dictionary<object, Hook>());
                m_Hooks.Add(hook, hooks);
            }

            (MainHook _, Dictionary<object, Hook> objectHooks) = hooks;

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
            {
                objectHook = new Hook();
                objectHooks.Add(o, objectHook);
            }

            objectHook.Event += action;
        }

        /// <summary>
        /// Subscribe a new action to the provided global-scoped hook
        /// </summary>
        public void Add(HookID hook, Action<object> action)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
            {
                hooks = (new MainHook(), new Dictionary<object, Hook>());
                m_Hooks.Add(hook, hooks);
            }

            (MainHook mainHook, Dictionary<object, Hook> _) = hooks;

            mainHook.Event += action;
        }

        /// <summary>
        /// Remove an action from the provided object-scoped hook
        /// </summary>
        public void Remove(HookID hook, Action action, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook _, Dictionary<object, Hook> objectHooks) = hooks;

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
                return;

            objectHook.Event -= action;
        }

        /// <summary>
        /// Remove an action from the provided global-scoped hook
        /// </summary>
        public void Remove(HookID hook, Action<object> action)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook mainHook, Dictionary<object, Hook> _) = hooks;

            mainHook.Event -= action;
        }
    }
}
