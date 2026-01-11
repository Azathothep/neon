namespace neon
{
    /// <summary>
    /// Wrapper around an <c>event Action`object</c>
    /// </summary>
    public class MainHook
    {
        public event Action<object> Event;

        public void Raise(object o) => Event?.Invoke(o);
    }
}
