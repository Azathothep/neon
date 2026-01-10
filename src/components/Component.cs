namespace neon
{
    public abstract class Component
    {
        public EntityID EntityID => m_EntityID;
        private EntityID m_EntityID = Entities.GetID(true);

        public abstract Component Clone();

        public EntityID Owner => Components.GetOwner(this);
        public T Add<T>() where T : Component, new() => this.Owner.Add<T>();
        public T Add<T>(T component) where T : Component => this.Owner.Add(component);
        public void Remove<T>() where T : Component => this.Owner.Remove<T>();
        public T Get<T>() where T : Component => this.Owner.Get<T>();
    }
}
