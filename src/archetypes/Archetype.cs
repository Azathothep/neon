using System;
using System.Collections.Generic;
using System.Data.Common;

namespace neon
{
    using Column = List<Component>;

    /// <summary>
    /// Provides links between neighbours archetypes (two archetypes are neighbours if they are different by only a single component)
    /// </summary>
    public struct ArchetypeEdges
    {
        public Archetype Add;
        public Archetype Remove;
    }

    /// <summary>
    /// Stores a specific set of <c>Component</c>.
    /// All entities with the same set components are stored together in an archetype.
    /// </summary>
    public class Archetype
    {
        public ArchetypeID ID => m_ID;
        private ArchetypeID m_ID;

        /// <summary>
        /// Contains the list of <c>ComponentID</c> stored by this <c>Archetype</c>
        /// </summary>
        public ComponentSet ComponentSet => m_ComponentSet;
        private ComponentSet m_ComponentSet;

        /// <summary>
        /// Stores the components
        /// </summary>
        public List<Column> Columns => m_Columns;
        private List<Column> m_Columns = new();

        /// <summary>
        /// References to existing neighbours <c>Archetypes</c>
        /// </summary>
        public Dictionary<ComponentID, ArchetypeEdges> Edges => m_Edges;
        private Dictionary<ComponentID, ArchetypeEdges> m_Edges = new();

        /// <summary>
        /// Every disabled entity are located before this index
        /// </summary>
        public int DisabledEndIndex => m_DisabledEndIndex;
        private int m_DisabledEndIndex;

        public int EntityCount => m_Columns[0].Count;

        public Archetype(ComponentSet componentSet)
        {
            m_ID = Archetypes.GetID();

            m_ComponentSet = componentSet;

            for (int i = 0; i < m_ComponentSet.ComponentIDs.Count; i++)
            {
                m_Columns.Add(new Column());
            }

            m_DisabledEndIndex = -1;
        }

        public int AddEntity(List<Component> components)
        {
            if (components.Count != m_Columns.Count)
            {
                // Something weird is happening
            }

            for (int i = 0; i < m_Columns.Count; i++)
            {
                m_Columns[i].Add(components[i]);
            }

            return m_Columns[0].Count - 1;
        }

        public List<Component> RemoveEntity(int row)
        {
            List<Component> components = new List<Component>();
            int maxIndex = m_Columns[0].Count - 1;

            for (int i = 0; i < m_Columns.Count; i++)
            {
                components.Add(m_Columns[i][row]);
                m_Columns[i][row] = m_Columns[i][maxIndex];
                m_Columns[i].RemoveAt(maxIndex);
            }

            return components;
        }

        public int OnEntityActiveStateChanged(int row, bool newActiveState)
        {
            int newIndex = row;

            if (newActiveState && row <= m_DisabledEndIndex)
            {
                newIndex = m_DisabledEndIndex;
                m_DisabledEndIndex--;
            }
            else if (newActiveState == false && row > m_DisabledEndIndex)
            {
                newIndex = m_DisabledEndIndex + 1;
                m_DisabledEndIndex++;
            }

			if (row != newIndex)
				SwapEntities(row, newIndex);

            return newIndex;
        }

        private void SwapEntities(int row1, int row2)
        {
            for (int i = 0; i < m_Columns.Count; i++)
            {
                Component temp = m_Columns[i][row1];
                m_Columns[i][row1] = m_Columns[i][row2];
                m_Columns[i][row2] = temp;
            }
        }
    }
}
