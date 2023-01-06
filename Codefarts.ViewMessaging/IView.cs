// <copyright file="IView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides a interface that represents a UI element.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        string ViewName { get; }

        /// <summary>
        /// Gets the unique identifier for the view.
        /// </summary>
        /// <remarks>Each view reference has a unique identifier.</remarks>
        string Id { get; }

        /// <summary>
        /// Gets the reference to the view object.
        /// </summary>
        object ViewReference { get; }
    }
}