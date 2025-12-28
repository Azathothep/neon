namespace neon
{
    public abstract class Component
    {
        public EntityID EntityID => m_EntityID;
        private EntityID m_EntityID = Entities.GetID(true);

        public abstract Component Clone();
    }
}
