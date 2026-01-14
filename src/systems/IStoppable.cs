namespace neon
{
    /// <summary>
    /// Implements the OnStop() method
    /// </summary>
    public interface IStoppable
    {
        /// <summary>
        /// Called when this <c>ISystem</c> is removed from the <c>SystemStorage</c>
        /// </summary>
        public void OnStop();
    }
}