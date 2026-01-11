namespace neon
{
    /// <summary>
    /// Provides global methods for creating, subscribing and unsubscribing to <c>Hooks</c>
    /// </summary>
    public static class Hooks
    {
        private static IHookStorage storage;

        /// <summary>
        /// Sets the underlying hook storage
        /// </summary>
        public static void SetStorage(IHookStorage storage)
        {
            Hooks.storage = storage;
        }

        public static HookTrigger<HookID> Create<HookID>(Type additionalType = null) where HookID : struct, IConvertible => storage.Create<HookID>(additionalType);

        public static HookTrigger<HookID> Create<HookID, T>() where HookID : struct, IConvertible => storage.Create<HookID, T>();

        public static void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible => storage.Add<HookID>(hook, action, o);

        public static void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible => storage.Add<HookID>(hook, action);

        public static void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible => storage.Add<HookID, T>(hook, action, o);

        public static void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible => storage.Add<HookID, T>(hook, action);

        public static void Remove<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible => storage.Remove<HookID>(hook, action, o);

        public static void Remove<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible => storage.Remove<HookID>(hook, action);

        public static void Remove<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible => storage.Remove<HookID, T>(hook, action, o);

        public static void Remove<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible => storage.Remove<HookID, T>(hook, action);
    }
}
