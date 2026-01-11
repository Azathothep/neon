namespace neon
{
    /// <summary>
    /// Wrapper around an <c>event Action</c>
    /// </summary>
    public class Hook
    {
        public event Action Event;

        public void Raise() => Event?.Invoke();
    }
}
