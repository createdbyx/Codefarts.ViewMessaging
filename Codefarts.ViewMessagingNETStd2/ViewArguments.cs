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


        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArguments"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary values that will be duplicated.</param>
        public ViewArguments(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewArguments"/> class.
        /// </summary>
        /// <param name="args">The args that will be duplicated.</param>
        public ViewArguments(ViewArguments args) : base(args)
        {
        }
    }
}
