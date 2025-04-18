using System.Diagnostics;
using System.Reflection;

namespace neon
{
    public class Systems
    {
        private class InternalStorage {
            public SystemStorage<IUpdateSystem> Update = new();
            public SystemStorage<IDrawSystem> Draw = new();
        }

        private static InternalStorage storage;

        private Systems() { }

        public static void Initialize() {
            storage = new();
        }

        public static void Add(IGameSystem gameSystem)
        {
            Type type = gameSystem.GetType();
            bool unique = type.GetCustomAttribute<AllowMultipleAttribute>() == null;

            if (unique && (storage.Update.Systems.FirstOrDefault(s => s.GetType() == type) != null
                || storage.Draw.Systems.FirstOrDefault(s => s.GetType() == type) != null))
            {
                Debug.WriteLine($"{type} has been added multiple times but is a unique system. Skipping.");
                return;
            }

            if (gameSystem is IUpdateSystem updateSystem)
                storage.Update.Add(updateSystem);

            if (gameSystem is IDrawSystem drawSystem)
                storage.Draw.Add(drawSystem);
        
        }

        public static void Remove(IGameSystem gameSystem)
        {
            if (gameSystem is IUpdateSystem updateSystem)
                storage.Update.Remove(updateSystem);

            if (gameSystem is IDrawSystem drawSystem)
                storage.Draw.Remove(drawSystem);
        }

        public static IGameSystem[] GetLoadedSystems()
        {
            List<IGameSystem> gameSystems = [.. storage.Update.Systems];

            foreach (IGameSystem system in storage.Draw.Systems)
            {
                if (gameSystems.Contains(system))
                    continue;

                gameSystems.Add(system);
            }

            return gameSystems.ToArray();
        }

        public static void Update(TimeSpan timeSpan)
        {
            for (int i = 0; i < storage.Update.Systems.Count; i++)
            {
                storage.Update.Systems[i].Update(timeSpan);
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < storage.Draw.Systems.Count; i++)
            {
                storage.Draw.Systems[i].Draw();
            }
        }
    }
}
