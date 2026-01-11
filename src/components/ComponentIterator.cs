using System.Diagnostics;

namespace neon
{
    /// <summary>
    /// Implements <c>IComponentIteractor</c>. Provides a way to request a fresh <c>QueryIteractor</c>
    /// </summary>
    public abstract class ComponentIterator : IComponentIterator
    {
        public bool IsDirty => m_IsDirty;
        protected bool m_IsDirty = true;

        /// <summary>
        /// Does the iterator includes or skips inactive entities
        /// </summary>
        protected bool m_IncludeInactive;

        protected Func<(Archetype, List<EntityID>)[]> m_RequestArchetypes;

        protected (Archetype, List<EntityID>)[] m_RequestedArchetypes;

        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive)
        {
            m_RequestArchetypes = requestArchetypes;
            m_IncludeInactive = includeInactive;
            Create();
        }

        /// <summary>
        /// Notifies this ComponentIterator to update its <c>Archetype</c> list
        /// </summary>
        public void SetDirty() => m_IsDirty = true;

        public IQueryIterator Create()
        {
            if (m_IsDirty)
            {
                // Debug.WriteLine("IsDirty: rebuilding");
                m_RequestedArchetypes = m_RequestArchetypes.Invoke();
                m_IsDirty = false;
            }

            return CreateInternal(m_RequestedArchetypes);
        }

        protected abstract IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 1-component Query.
    /// </summary>
    public class ComponentIterator<T> : ComponentIterator where T : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T>(archetypes, m_IncludeInactive);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 2-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2> : ComponentIterator where T1 : Component where T2 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2>(archetypes, m_IncludeInactive);

    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 3-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2, T3> : ComponentIterator where T1 : Component where T2 : Component where T3 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3>(archetypes, m_IncludeInactive);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 4-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2, T3, T4> : ComponentIterator where T1 : Component where T2 : Component where T3 : Component where T4 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3, T4>(archetypes, m_IncludeInactive);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 5-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2, T3, T4, T5> : ComponentIterator where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3, T4, T5>(archetypes, m_IncludeInactive);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 6-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2, T3, T4, T5, T6> : ComponentIterator where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3, T4, T5, T6>(archetypes, m_IncludeInactive);
    }

    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c> for 7-components Query.
    /// </summary>
    public class ComponentIterator<T1, T2, T3, T4, T5, T6, T7> : ComponentIterator where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3, T4, T5, T6, T7>(archetypes, m_IncludeInactive);
    }
}
