namespace neon
{
    /// <summary>
    /// Stores and interacts with <c>HookMaps</c>.
    /// Hooks can be global-scoped or object-scoped. They are always linked to a target object (entity, component...).
    /// A global-scoped hook is called when the hook is raised, independently of the target object.
    /// A object-scoped hook is called on a specific object when the hook is raised on it. 
    /// </summary>
    public interface IHookStorage
    {
        /// <summary>
        /// Creates a new <c>HookMap</c> with the specified <c>HookID</c> and returns its corresponding <c>HookTrigger</c>.
        /// You can provide an additional type to restraint the hook scope (useful for <c>Component</c> hooks).
        /// </summary>
        public HookTrigger<HookID> Create<HookID>(Type additionalType = null) where HookID : struct, IConvertible;

        /// <summary>
        /// Creates a new <c>HookMap</c> with the specified <c>HookID</c> and returns its corresponding <c>HookTrigger</c>.
        /// </summary>
        public HookTrigger<HookID> Create<HookID, T>() where HookID : struct, IConvertible;

        /// <summary>
        /// Subscribe to the specified HookID in object-scope
        /// </summary>
        public void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        /// <summary>
        /// Subscribe to the specified HookID in global-scope
        /// </summary>
        public void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        /// <summary>
        /// Subscribe to the specified type-restrained HookID in object-scope
        /// </summary>
        public void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        /// <summary>
        /// Subscribe to the specified type-restrained HookID in global-scope
        /// </summary>
        public void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        /// <summary>
        /// Unsubscribe to the specified HookID targetting provided object
        /// </summary>
        public void Remove<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        /// <summary>
        /// Unsubscribe to the specified HookID in global-scope
        /// </summary>
        public void Remove<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        /// <summary>
        /// Unsubscribe to the specified type-restrained HookID targetting provided object
        /// </summary>
        public void Remove<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        /// <summary>
        /// Unsubscribe to the specified type-restrained HookID in global-scope
        /// </summary>
        public void Remove<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

    }
}
