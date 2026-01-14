using System.Collections;

namespace neon
{
    /// <summary>
    /// Provides a way to request a fresh <c>QueryIterator</c>
    /// </summary>
    public interface IComponentIterator
    {
        public bool IsDirty { get; }

        /// <summary>
        /// Notifies when this ComponentIterator needs to rebuild its underlying storage. 
        /// </summary>
        public void SetDirty();

        public IQueryIterator Create();
    }
}
