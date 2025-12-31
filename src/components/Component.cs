namespace neon
{
    public abstract class Component
    {
        public EntityID EntityID => m_EntityID;
        private EntityID m_EntityID = Entities.GetID(true);

        public abstract Component Clone();

        public T Add<T>() where T : Component, new() => Components.GetOwner(this).Add<T>();
        public T Add<T>(T component) where T : Component => Components.GetOwner(this).Add(component);
        public void Remove<T>() where T : Component => Components.GetOwner(this).Remove<T>();
        public T Get<T>() where T : Component => Components.GetOwner(this).Get<T>();
    }
}
