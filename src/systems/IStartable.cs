namespace neon
{
    /// <summary>
    /// Implements the OnStart() method
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// Called when this <c>ISystem</c> is added to the <c>SystemStorage</c>
        /// </summary>
        public void OnStart();
    }
}