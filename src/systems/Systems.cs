using System.Diagnostics;
using System.Reflection;

namespace neon
{
    /// <summary>
    /// Provides a way to update, add and remove systems from the storage
    /// </summary>
    public static class Systems
    {
        private class InternalStorage {
            public SystemStorage<IUpdateSystem> Update = new();
            public SystemStorage<IDrawSystem> Draw = new();
        }

        private static InternalStorage storage;

        /// <summary>
        /// Initialize the underlying system storage
        /// </summary>
        public static void Initialize() {
            storage = new();
        }

        /// <summary>
        /// Add an active system to the storage. Its underlying system method will be called periodically.
        /// </summary>
        public static void Add(ISystem system)
        {
            Type type = system.GetType();
            bool unique = type.GetCustomAttribute<AllowMultipleAttribute>() == null;

            if (unique && (storage.Update.Systems.FirstOrDefault(s => s.GetType() == type) != null
                || storage.Draw.Systems.FirstOrDefault(s => s.GetType() == type) != null))
            {
                Debug.WriteLine($"{type} has been added multiple times but is a unique system. Skipping.");
                return;
            }

            if (system is IUpdateSystem updateSystem)
                storage.Update.Add(updateSystem);

            if (system is IDrawSystem drawSystem)
                storage.Draw.Add(drawSystem);
        
            if (system is IStartable startable)
                startable.OnStart();
        }

        /// <summary>
        /// Remove an active system from the storage. Its underlying system method won't be called anymore.
        /// </summary>
        public static void Remove(ISystem system)
        {
            if (system is IUpdateSystem updateSystem)
                storage.Update.Remove(updateSystem);

            if (system is IDrawSystem drawSystem)
                storage.Draw.Remove(drawSystem);

            if (system is IStoppable stoppable)
                stoppable.OnStop();
        }

        /// <summary>
        /// Get the currently stored systems
        /// </summary>
        public static ISystem[] GetLoadedSystems()
        {
            List<ISystem> gameSystems = [.. storage.Update.Systems];

            foreach (ISystem system in storage.Draw.Systems)
            {
                if (gameSystems.Contains(system))
                    continue;

                gameSystems.Add(system);
            }

            return gameSystems.ToArray();
        }

        /// <summary>
        /// Call the Update() method of every <c>IUpdateSystem</c> stored
        /// </summary>
        public static void Update(TimeSpan timeSpan)
        {
            for (int i = 0; i < storage.Update.Systems.Count; i++)
            {
                storage.Update.Systems[i].Update(timeSpan);
            }
        }

        /// <summary>
        /// Call the Draw() method of every <c>IDrawSystem</c> stored
        /// </summary>
        public static void Draw()
        {
            for (int i = 0; i < storage.Draw.Systems.Count; i++)
            {
                storage.Draw.Systems[i].Draw();
            }
        }
    }
}
