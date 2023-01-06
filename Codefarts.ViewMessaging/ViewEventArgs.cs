// <copyright file="ViewEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    /// <summary>
    /// Provides event arguments for a view.
    /// </summary>
    public class ViewEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEventArgs"/> class.
        /// </summary>
        /// <param name="view">A reference to a <see cref="IView"/> implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> parameter is null.</exception>
        public ViewEventArgs(IView view)
        {
            this.View = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Gets the reference to the view.
        /// </summary>
        public IView View
        {
            get;
        }
    }
}