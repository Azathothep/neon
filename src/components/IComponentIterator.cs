using System.Collections;

namespace neon
{
    /// <summary>
    /// Provides a way to request a fresh <c>QueryIteractor</c>
    /// </summary>
    public interface IComponentIterator
    {
        public bool IsDirty { get; }

        public void SetDirty();

        public IQueryIterator Create();
    }
}
