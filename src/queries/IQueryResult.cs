namespace neon
{
    /// <summary>
    /// Contains the result of an <c>IQuery</c>
    /// </summary>
    public interface IQueryResult
    {
        /// <summary>
        /// Set when the ComponentStorage has updated a component type specified in the related Query.
        /// </summary>
        public bool IsDirty { get; }

        public void SetDirty();
    }
}
