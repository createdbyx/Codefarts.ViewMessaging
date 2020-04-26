#if !NET20
namespace Codefarts.ViewMessaging
{
    public static class ExtensionMethods
    {
        public static T Get<T>(this ViewArguments args, string key, T fallbackValue)
        {
            object value;
            if (args.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return fallbackValue;
        }

        public static T Get<T>(this ViewArguments args, string key)
        {
            return args.Get(key, default(T));
        }
    }
}
#endif