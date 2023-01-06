// <copyright file="GenericMessageArguments.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides helper methods for generating predefined view arguments.
    /// </summary>
    public class GenericMessageArguments
    {
        /// <summary>
        /// Groups multiple view arguments.
        /// </summary>
        /// <param name="arguments">The collection of view arguments to be combined.</param>
        /// <returns>A new <see cref="ViewArguments"/> reference containing the combined kay value pairs.</returns>
        /// <remarks>NOTE: That later additions with matching key names will overwrite existing entries.</remarks>
        public static ViewArguments Build(params IDictionary<string, object>[] arguments)
        {
            var args = new Dictionary<string, object>();
            foreach (var dictionary in arguments)
            {
                foreach (var pair in dictionary)
                {
                    args[pair.Key] = pair.Value;
                }
            }

            return new ViewArguments(args);
        }

        /// <summary>
        /// Builds simple show arguments.
        /// </summary>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        public static ViewArguments Show()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Show] = string.Empty;
            return new ViewArguments(arguments);
        }

        /// <summary>
        /// Builds simple show dialog arguments.
        /// </summary>
        /// <param name="viewId">A view id that represents the parent dialog.</param>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        public static ViewArguments ShowDialog(string viewId)
        {
            if (viewId == null)
            {
                throw new ArgumentNullException(nameof(viewId));
            }

            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.ViewId] = viewId;
            return new ViewArguments(arguments);
        }

        /// <summary>
        /// Builds set model arguments.
        /// </summary>
        /// <param name="model">A reference to a object.</param>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        public static ViewArguments SetModel(object model)
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.SetModel] = model;
            return new ViewArguments(arguments);
        }

        /// <summary>
        /// Builds set parent arguments.
        /// </summary>
        /// <param name="parent">A reference to a parent object.</param>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        public static ViewArguments SetParent(object parent)
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.SetParent] = parent;
            return new ViewArguments(arguments);
        }

        /// <summary>
        /// Builds update arguments.
        /// </summary>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        /// <remarks>Intended for scenarios like a game engine where game components need to be sent Draw or Update commands.</remarks>
        public static ViewArguments Update()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Update] = default;
            return new ViewArguments(arguments);
        }

        /// <summary>
        /// Builds refresh arguments.
        /// </summary>
        /// <returns>A reference to a new <see cref="ViewArguments"/> type.</returns>
        public static ViewArguments Refresh()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Refresh] = default;
            return new ViewArguments(arguments);
        }
    }
}