#if NET20     
namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides a readonly collection of named arguments.
    /// </summary>
    public partial class ViewArguments
    {
        public T Get<T>(string key, T fallbackValue)
        {
            object value;
            if (this.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return fallbackValue;
        }

        public T Get<T>(string key)
        {
            return this.Get(key, default(T));
        }
    }
}

#endif