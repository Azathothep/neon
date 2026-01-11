using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    /// <summary>
    /// Interface container for EntityIDs
    /// </summary>
    public interface IEntityStorage
    {
        /// <summary>
        /// Get a new unique EntityID
        /// </summary>
        public EntityID GetID(bool isComponent = false);
       
        /// <summary>
        /// Destroy the provided entity and all of its children and components
        /// </summary>
        public void Destroy(EntityID entityID);

        /// <summary>
        /// Set a parent / child relationship between two entities
        /// </summary>
        public void SetRelation(EntityID parentID, EntityID childID);

        /// <summary>
        /// Get entities at the root of the scene (without a parent)
        /// </summary>
        public EntityID[] GetRoots();

        public EntityID GetParent(EntityID entityID);

        /// <summary>
        /// Get all children of the provided entity. As components also are entities, you can specify if they are included in the array or not.
        /// </summary>
        public EntityID[] GetChildren(EntityID entityID, bool includeComponents = true);

        /// <summary>
        /// Update the active state of the provided entity's children relative to their parent
        /// </summary>
        public void UpdateState(EntityID entityID);
    }
}
