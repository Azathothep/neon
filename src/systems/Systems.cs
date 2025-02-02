using System.Diagnostics;
using System.Reflection;

namespace neon
{
    public class Systems
    {
        private class SystemsStorage
        {
            public List<IUpdateSystem> UpdateSystems = new();
            public List<IDrawSystem> DrawSystems = new();
        }

        private static SystemsStorage storage = new();

        private Systems() { }

        public static void Add(IGameSystem gameSystem)
        {
            Type type = gameSystem.GetType();
            bool unique = type.GetCustomAttribute<AllowMultipleAttribute>() == null;

            if (unique && (storage.UpdateSystems.FirstOrDefault(s => s.GetType() == type) != null
                || storage.DrawSystems.FirstOrDefault(s => s.GetType() == type) != null))
            {
                Debug.WriteLine($"{type} has been added multiple times but is a unique system. Skipping.");
                return;
            }

            if (gameSystem is IUpdateSystem)
                storage.UpdateSystems.Add((IUpdateSystem)gameSystem);
            
            if (gameSystem is IDrawSystem)
                storage.DrawSystems.Add((IDrawSystem)gameSystem);
        }

        public static void Remove(IGameSystem gameSystem)
        {
            if (gameSystem is IUpdateSystem)
                storage.UpdateSystems.Remove((IUpdateSystem)gameSystem);
            
            if (gameSystem is IDrawSystem)
                storage.DrawSystems.Remove((IDrawSystem)gameSystem);
        }

        public static IGameSystem[] GetLoadedSystems()
        {
            List<IGameSystem> gameSystems = new List<IGameSystem>();

            foreach (IGameSystem system in storage.UpdateSystems)
                gameSystems.Add(system);

            foreach (IGameSystem system in storage.DrawSystems)
            {
                if (gameSystems.Contains(system))
                    continue;

                gameSystems.Add(system);
            }

            return gameSystems.ToArray();
        }

        public static void Update(TimeSpan timeSpan)
        {
            for (int i = 0; i < storage.UpdateSystems.Count; i++)
            {
                storage.UpdateSystems[i].Update(timeSpan);
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < storage.DrawSystems.Count; i++)
            {
                storage.DrawSystems[i].Draw();
            }
        }
    }
}
