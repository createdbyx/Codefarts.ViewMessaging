namespace Codefarts.ViewMessaging
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a readonly collection of named arguments.
    /// </summary>
    public class ViewArguments : ReadOnlyDictionary<string, object>
    {
        /// <summary>Initializes a new instance of the <see cref="ViewArguments" /> class.</summary>
        public ViewArguments() : base(new Dictionary<string, object>())
        {
        }

        /// <inheritdoc />
        public ViewArguments(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        public ViewArguments(ViewArguments args) : base(args)
        {
        }

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
