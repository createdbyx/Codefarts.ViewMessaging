// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

#if !NET20
namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides extension methods for the <see cref="ViewArguments"/> type.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the argument value from a <see cref="ViewArguments"/> type.
        /// </summary>
        /// <param name="args">The arguments container.</param>
        /// <param name="key">The name of the argument to be retrieved.</param>
        /// <param name="fallbackValue">The fallback value to return is there was a problem retrieving the argument.</param>
        /// <typeparam name="T">The type to convert the argument value into.</typeparam>
        /// <returns>Returns the converted argument value; otherwise if something went wrong returns the fallback value.</returns>
        public static T Get<T>(this ViewArguments args, string key, T fallbackValue)
        {
            object value;
            if (args.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return fallbackValue;
        }

        /// <summary>
        /// Gets the argument value from a <see cref="ViewArguments"/> type.
        /// </summary>
        /// <param name="args">The arguments container.</param>
        /// <param name="key">The name of the argument to be retrieved.</param>
        /// <typeparam name="T">The type to convert the argument value into.</typeparam>
        /// <returns>Returns the converted argument value.</returns>
        /// <remarks>This method may throw an exception if the argument value could not be converted or some unexpected exception was thrown.</remarks>
        public static T Get<T>(this ViewArguments args, string key)
        {
            return args.Get(key, default(T));
        }
    }
}
#endif