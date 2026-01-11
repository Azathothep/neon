using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace neon
{
    // The component storage architecture has been created following Sander Mertens' (creator or FLECS) "Building an ECS" article series
    // You can read the first one here : https://ajmmertens.medium.com/building-an-ecs-1-where-are-my-entities-and-components-63d07c7da742

    /// <summary>
    /// Implements <c>IComponentStorage</c>. Container for any <c>Component</c> added to an entity.
    /// </summary>
    public class ComponentStorage : IComponentStorage
    {
        /// <summary>
        /// Creates and caches <c>IComponentIterators</c> relative to queries
        /// </summary>
        public class ComponentIteratorProvider : IComponentIteratorProvider
        {
            private Dictionary<ComponentID, HashSet<IComponentIterator>> m_IterablesCollection = new();

            private ComponentStorage m_Storage;

            public ComponentIteratorProvider(ComponentStorage componentStorage)
            {
                m_Storage = componentStorage;
                m_Storage.m_OnArchetypeAdded += SetDirty;
            }

            /// <summary>
            /// Sets every <c>ComponentIterator</c> linked to any component of the provided <c>ComponentSet</c> as dirty
            /// </summary>
            public void SetDirty(ComponentSet componentSet)
            {
                for (int i = 0; i < componentSet.ComponentIDs.Count; i++)
                {
                    ComponentID componentID = componentSet.ComponentIDs[i];
                    if (m_IterablesCollection.TryGetValue(componentID, out HashSet<IComponentIterator> iterableSet))
                    {
                        foreach (var iterable in iterableSet)
                            iterable.SetDirty();
                    }
                }
            }

            public IComponentIterator Get<T>(IQuery query, QueryType queryType) where T : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3, T4>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3, T4>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3, T4, T5>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3, T4, T5>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3, T4, T5, T6>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3, T4, T5, T6>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            public IComponentIterator Get<T1, T2, T3, T4, T5, T6, T7>(IQuery query, QueryType queryType) where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component
            {
                IComponentIterator iterableQuery = new ComponentIterator<T1, T2, T3, T4, T5, T6, T7>(() => RequestArchetypes(query.Filters), query.IncludeInactive);
                if (queryType == QueryType.Cached)
                    AddIterableToCollection(query.Filters, iterableQuery);
                return iterableQuery;
            }

            private void AddIterableToCollection(IQueryFilter[] filters, IComponentIterator iterableQuery)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    if (!m_IterablesCollection.TryGetValue(filters[i].ComponentID, out HashSet<IComponentIterator> iterableSet))
                    {
                        iterableSet = new HashSet<IComponentIterator>();
                        m_IterablesCollection.Add(filters[i].ComponentID, iterableSet);
                    }

                    iterableSet.Add(iterableQuery);
                }
            }

            /// <summary>
            /// Get all Archetypes satisfying the provided <c>QueryFilters</c>
            /// </summary>
            private (Archetype, List<EntityID>)[] RequestArchetypes(IQueryFilter[] queryFilters)
            {
                List<ArchetypeID> archetypesLeft = new();

                int filterIndex = 0;

                // Filters are sorted by the term in the provided array. First FilterTerm.Has, then FilterTerm.HasNot, and finally FilterTerm.MightHave.
                // The filtering process will start from the list of all archetypes, then remove the ones that do not respect the filters.

                // If the first filter term is FilterTerm.Has, we can initialize the ArchetypeID list with only the ones that have the associated ComponentID.
                if (queryFilters[0].Term == FilterTerm.Has)
                {
                    if (m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[0].ComponentID, out Dictionary<ArchetypeID, int> satisfyingArchetypes))
                    {
                        foreach (var a in satisfyingArchetypes)
                            archetypesLeft.Add(a.Key);
                    }

                    filterIndex = 1;
                }
                else
                {
                    foreach ((ArchetypeID id, Archetype _) in m_Storage.m_ArchetypeIDToArchetype)
                        archetypesLeft.Add(id);
                }

                // As long as the filter term is FilterTerm.Has, remove all archetypes that have not the associated ComponentID

                for (; filterIndex < queryFilters.Length; filterIndex++)
                {
                    if (queryFilters[filterIndex].Term != FilterTerm.Has)
                        break;

                    Dictionary<ArchetypeID, int> satisfyingArchetypes;

                    if (!m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[filterIndex].ComponentID, out satisfyingArchetypes))
                        satisfyingArchetypes = new();

                    for (int j = archetypesLeft.Count - 1; j >= 0; j--)
                    {
                        if (satisfyingArchetypes.ContainsKey(archetypesLeft[j]) == false)
                            archetypesLeft.RemoveAt(j);
                    }
                }

                // As long as the filter term is FilterTerm.HasNot, remove all archetypes that have the associated ComponentID

                for (; filterIndex < queryFilters.Length; filterIndex++)
                {
                    if (queryFilters[filterIndex].Term != FilterTerm.HasNot)
                        break;

                    Dictionary<ArchetypeID, int> satisfyingArchetypes;

                    if (!m_Storage.m_ComponentIDToArchetypeSet.TryGetValue(queryFilters[filterIndex].ComponentID, out satisfyingArchetypes))
                        satisfyingArchetypes = new();

                    for (int j = archetypesLeft.Count - 1; j >= 0; j--)
                    {
                        if (satisfyingArchetypes.ContainsKey(archetypesLeft[j]))
                            archetypesLeft.RemoveAt(j);
                    }
                }

                // No need to check for FilterTerm.MightHave, as it indicates the Component is optional (the archetype list won't be affected)

                List<(Archetype, List<EntityID>)> archetypes = new();

                for (int j = 0; j < archetypesLeft.Count; j++)
                {
                    Archetype archetype = m_Storage.m_ArchetypeIDToArchetype[archetypesLeft[j]];
                    List<EntityID> entities = m_Storage.m_ArchetypeToEntities[archetype.ID];

                    archetypes.Add((archetype, entities));
                }

                return archetypes.ToArray();
            }
        }

        private Dictionary<EntityID, (Archetype, int)> m_EntityToArchetype = new();
        private Dictionary<ArchetypeID, List<EntityID>> m_ArchetypeToEntities = new();
        private Dictionary<ComponentSet, Archetype> m_ComponentSetToArchetype = new();
        private Dictionary<ComponentID, Dictionary<ArchetypeID, int>> m_ComponentIDToArchetypeSet = new();
        private Dictionary<ArchetypeID, Archetype> m_ArchetypeIDToArchetype = new();
        private event Action<ComponentSet> m_OnArchetypeAdded;

        private ComponentStorageNotifier m_ComponentStorageNotifier;

        private IComponentIteratorProvider m_IteratorProvider;
        public IComponentIteratorProvider IteratorProvider => m_IteratorProvider;

        private Dictionary<ComponentID, HookTrigger<ComponentHook>> m_HookTriggers = new();

        public ComponentStorage(ComponentStorageNotifier storageNotifier)
        {
            m_ComponentStorageNotifier = storageNotifier;

            m_IteratorProvider = new ComponentIteratorProvider(this);

            Hooks.Add(EntityHook.OnEnabled, (o) => OnEntityActiveStateChanged((EntityID)o, true));
            Hooks.Add(EntityHook.OnDisabled, (o) => OnEntityActiveStateChanged((EntityID)o, false));
        }

        /// <summary>
        /// Get a specific <c>Component</c> attached to the provided entity
        /// </summary>
        public T Get<T>(EntityID entityID) where T : Component
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                // The entity doesn't have any component yet
                //Debug.WriteLine($"Error: Archetype for entity {entityID} not found in dictionary");
                return null;
            }

            (Archetype archetype, int row) = archetypeRecord;

            int column = GetColumn(componentID, archetype);

            if (column == -1)
            {
                // Debug.WriteLine("Something Happened");
                return null;
            }

            return (T)archetype.Columns[column][row];
        }

        /// <summary>
        /// Get all <c>Components</c> attached to the provided entity
        /// </summary>
		public Component[] GetAll(EntityID entityID)
		{
			if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
			{
				return new Component[0];
			}

			(Archetype archetype, int rows) = archetypeRecord;

			int count = archetype.Columns.Count;

			Component[] components = new Component[count];

			for (int i = 0; i < count; i++)
				components[i] = archetype.Columns[i][rows];

			return components;
		}

        /// <summary>
        /// Request if the provided entity has a specific <c>Component</c>
        /// </summary>
        public bool Has<T>(EntityID entityID) where T : Component
        {
            ComponentID componentID = Components.GetID<T>();

            // Getting entity's archetype & row index
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                Debug.WriteLine($"Error: Archetype for entity {entityID} not found in dictionary");
                return false;
            }

            (Archetype archetype, int _) = archetypeRecord;

            // Getting every archetypeID that contains the required component & the component position in their column
            if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
            {
                Debug.WriteLine($"Error: ArchetypeSet for component {componentID} not found in dictionary");
                return false;
            }

            // if entity's archetype ID is in archetypeID set, entity has the component
            return archetypeSet.ContainsKey(archetype.ID);
        }

        /// <summary>
        /// Add a clone of the provided <c>Component</c> to the provided entity
        /// </summary>
        public T Add<T>(EntityID entityID, T component) where T : Component
        {
            return (T)Add(entityID, component, typeof(T));
        }

        /// <summary>
        /// Add a clone of the provided <c>Component</c> to the provided entity
        /// </summary>
        public Component Add(EntityID entityID, Component component, Type type)
        {
            // Debug.WriteLine($"Adding component of type {type} to {(UInt32)entityID}");

            component.EntityID.SetParent(entityID);

            if (component.GetType().IsAssignableFrom(typeof(Component)))
                return null;

            ComponentID componentID = Components.GetIDByType(type);

            if (componentID == null)
                return null;

            // If the entity isn't recorded yet for having any component, add it to an Archetype
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
            {
                ComponentSet componentSet = new ComponentSet(componentID);

                Archetype archetype = GetOrCreateArchetype(componentSet);

                AddEntityToArchetype(entityID, new List<Component> { component }, archetype);

                OnComponentAdded(component, componentID);

                return component;
            }

            {
                (Archetype archetype, int row) = archetypeRecord;

                {
                    // If the entity already has this component
                    if (archetype.ComponentSet.ComponentIDs.Contains(componentID))
                    {
                        int column = GetColumn(componentID, archetype);
                        Component previousComponent = archetype.Columns[column][row];

                        archetype.Columns[column][row] = component; // Replace the component

                        Destroy(previousComponent);

                        return component;
                    }
                }

                {
                    if (archetype == null)
                        throw new ArgumentException($"Archetype for {componentID} is null");

                    // If the entity already exists in an Archetype, move it to the neighbour archetype containing the same ComponentSet plus the new component type

                    if (!archetype.Edges.TryGetValue(componentID, out ArchetypeEdges archetypeEdges))
                    {
                        archetypeEdges = new ArchetypeEdges();
                    }

                    Archetype nextArchetype = archetypeEdges.Add;

                    // If Edges.Add hasn't been set yet
                    if (nextArchetype == null)
                    {
                        ComponentSet newComponentSet = archetype.ComponentSet.Add(componentID);

                        nextArchetype = GetOrCreateArchetype(newComponentSet);

                        archetypeEdges.Add = nextArchetype;
                        archetype.Edges.Add(componentID, archetypeEdges);
                    }

                    // Removing components from archetype, add it to the list, then to the nextArchetype and save result

                    List<Component> components = RemoveEntityFromArchetype(entityID, archetype, row);

                    int column = GetColumn(componentID, nextArchetype);

                    components.Insert(column, component); // This should stay in order, because we're inserting it at its right location

                    AddEntityToArchetype(entityID, components, nextArchetype);
                }
            }

            OnComponentAdded(component, componentID);

            return component;
        }

        private void OnComponentAdded(Component component, ComponentID componentID)
        {
            if (component is IAwakable awakable)
                awakable.Awake();

            Trigger(ComponentHook.OnAdded, componentID, component.GetType(), component.EntityID);
        }

        /// <summary>
        /// Remove the specified <c>Component</c> from the provided entity
        /// </summary>
        public void Remove<T>(EntityID entityID) where T : Component
        {
            ComponentID componentID = Components.GetID<T>();

            // If the entity isn't recorded yet for having any component
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
                return;

            (Archetype archetype, int row) = archetypeRecord;

            if (archetype == null)
                throw new ArgumentException($"Archetype for {componentID} is null");

            // if the archetype doesn't have this component
            if (!archetype.ComponentSet.ComponentIDs.Contains(componentID))
                return;

            // if the archetype contains only one (this) component, only remove it from the archetype
            if (archetype.ComponentSet.ComponentIDs.Count == 1)
            {
                Component component = RemoveEntityFromArchetype(entityID, archetype, row)[0];
                m_EntityToArchetype.Remove(entityID);
                Destroy(component);

                Trigger(ComponentHook.OnRemoved, componentID, typeof(T), entityID);

                return;
            }

            // else, if the archetype contains other components, downgrade the entity to the neighbour archetype containing the same ComponentSet minus the removed component type

            if (!archetype.Edges.TryGetValue(componentID, out ArchetypeEdges archetypeEdges))
            {
                archetypeEdges = new ArchetypeEdges();
            }

            Archetype nextArchetype = archetypeEdges.Remove;

            // If Edges.Remove hasn't been set yet
            if (nextArchetype == null)
            {
                ComponentSet newComponentSet = archetype.ComponentSet.Remove(componentID);

                nextArchetype = GetOrCreateArchetype(newComponentSet);

                archetypeEdges.Remove = nextArchetype;
                archetype.Edges.Add(componentID, archetypeEdges);
            }

            // Removing component from archetype & list, then adding it to nextArchetype & save result
            {
                List<Component> components = RemoveEntityFromArchetype(entityID, archetype, row);

                int column = GetColumn(componentID, archetype);

                Component component = components[column];

                components.RemoveAt(column); // This will remove the component set at column

                Destroy(component);

                AddEntityToArchetype(entityID, components, nextArchetype);
            }

            Trigger(ComponentHook.OnRemoved, componentID, typeof(T), entityID);
        }

        /// <summary>
        /// Remove all components attached to the provided entity
        /// </summary>
        public void Remove(EntityID entityID)
        {
            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) value))
                return;

            Archetype archetype = value.Item1;
            int row = value.Item2;

            List<Component> components = RemoveEntityFromArchetype(entityID, archetype, row);

            for (int i = 0; i < components.Count; i++)
            {
                Destroy(components[i]);
            }
        }

        public object[] GetComponentsFromIDs(EntityID entityID, ComponentID[] componentIDs)
        {
            object[] components = new object[componentIDs.Length];

            (Archetype, int) archetypeRecord = m_EntityToArchetype[entityID];

            Archetype archetype = archetypeRecord.Item1;
            int row = archetypeRecord.Item2;

            for (int i = 0; i < componentIDs.Length; i++)
            {
                Dictionary<ArchetypeID, int> componentArchetypes = m_ComponentIDToArchetypeSet[componentIDs[i]];

                if (componentArchetypes.TryGetValue(archetype.ID, out int column))
                    components[i] = archetype.Columns[column][row];
            }

            return components;
        }

        private int GetColumn(ComponentID componentID, Archetype archetype)
        {
            // Getting every archetypeID that contains the required component & the component position in their column
            if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                return -1;

            // Getting entity's archetype column for required component
            // (if false, archetype - consequently, the entity - does not posess the required component)
            if (!archetypeSet.TryGetValue(archetype.ID, out int column))
                return -1;

            return column;
        }

        private Archetype GetOrCreateArchetype(ComponentSet componentSet)
        {
            // if there is no archetype registered for this ComponentSet
            if (!m_ComponentSetToArchetype.TryGetValue(componentSet, out Archetype archetype))
            {
                archetype = new Archetype(componentSet);
                m_ComponentSetToArchetype.Add(componentSet, archetype);
                m_ArchetypeIDToArchetype.Add(archetype.ID, archetype);

                m_OnArchetypeAdded.Invoke(componentSet);

                // foreach componentID in componentSet, add lookup data to the componenID-to-archetype-infos dictionary
                for (int i = 0; i < componentSet.ComponentIDs.Count; i++)
                {
                    ComponentID componentID = componentSet.ComponentIDs[i];

                    // if archetypeSet not yet exists (= first time this componentID is brought to it), create a new one

                    if (!m_ComponentIDToArchetypeSet.TryGetValue(componentID, out Dictionary<ArchetypeID, int> archetypeSet))
                    {
                        archetypeSet = new Dictionary<ArchetypeID, int>();
                        m_ComponentIDToArchetypeSet.Add(componentID, archetypeSet);
                    }

                    archetypeSet.Add(archetype.ID, i);
                }
            }

            return archetype;
        }

        private void AddEntityToArchetype(EntityID entityID, List<Component> components, Archetype archetype)
        {
            int row = archetype.AddEntity(components);

            m_EntityToArchetype[entityID] = (archetype, row);

            if (!m_ArchetypeToEntities.TryGetValue(archetype.ID, out List<EntityID> entities))
            {
                entities = new List<EntityID>();
                m_ArchetypeToEntities.Add(archetype.ID, entities);
            }

            entities.Add(entityID);

            foreach (var cID in archetype.ComponentSet.ComponentIDs)
            {
                m_ComponentStorageNotifier.Raise(cID);
            }
        }

        private List<Component> RemoveEntityFromArchetype(EntityID entityID, Archetype archetype, int row)
        {
            List<Component> components = archetype.RemoveEntity(row);

            List<EntityID> entities = m_ArchetypeToEntities[archetype.ID];
            int maxIndex = entities.Count - 1;
            EntityID lastEntity = entities[maxIndex];

            m_EntityToArchetype[lastEntity] = (archetype, row);

            m_ArchetypeToEntities[archetype.ID][row] = lastEntity;
            m_ArchetypeToEntities[archetype.ID].RemoveAt(maxIndex);

            foreach (var cID in archetype.ComponentSet.ComponentIDs)
            {
                m_ComponentStorageNotifier.Raise(cID);
            }

            return components;
        }

        /// <summary>
        /// Get the entity the provided component is attached to
        /// </summary>
        public EntityID GetOwner<T>(T component) where T : Component => component.EntityID.GetParent();

        private void Destroy(Component component)
        {
            Entities.Destroy(component.EntityID);
        }

        /// <summary>
        /// Enables or disables all components attached to the provided entity, in reaction to its state
        /// </summary>
        private void OnEntityActiveStateChanged(EntityID entityID, bool newActiveState)
        {
            if (entityID.isComponent)
                entityID = entityID.GetParent();

            if (!m_EntityToArchetype.TryGetValue(entityID, out (Archetype, int) archetypeRecord))
                return;

            (Archetype archetype, int row) = archetypeRecord;

            int newIndex = archetype.OnEntityActiveStateChanged(row, newActiveState);
            if (newIndex != row)
            {
                EntityID other = m_ArchetypeToEntities[archetype.ID][newIndex];

                m_EntityToArchetype[entityID] = (archetype, newIndex);
                m_EntityToArchetype[other] = (archetype, row);
            }

            foreach (var cID in archetype.ComponentSet.ComponentIDs)
            {
                m_ComponentStorageNotifier.Raise(cID);
            }
        }

        public T[] GetInChildren<T>(EntityID entityID, bool propagate = false) where T : Component
        {
            List<T> components = new List<T>();

            EntityID[] children = entityID.GetChildren(false);

            foreach (var child in children)
            {
                T component = child.Get<T>();
                if (component != null)
                    components.Add(component);

                if (propagate)
                    components.AddRange(GetInChildren<T>(child, propagate));
            }

            return components.ToArray();
        }

        public T[] GetInParents<T>(EntityID entityID) where T : Component
        {
            List<T> components = new List<T>();

            EntityID parent = entityID.GetParent();

            while (parent != null)
            {
                T component = parent.Get<T>();
                if (component != null)
                    components.Add(component);

                parent = parent.GetParent();
            }

            return components.ToArray();
        }

        /// <summary>
        /// Triggers ComponentHook events in reaction to a component addition or deletion
        /// </summary>
        private void Trigger(ComponentHook hook, ComponentID id, Type componentType, object o)
        {
            if (!m_HookTriggers.TryGetValue(id, out HookTrigger<ComponentHook> hookTrigger))
            {
                hookTrigger = Hooks.Create<ComponentHook>(componentType);
                m_HookTriggers.Add(id, hookTrigger);
            }

            hookTrigger.Raise(hook, o);
        }
    }
}
