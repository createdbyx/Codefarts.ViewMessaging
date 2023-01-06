// <copyright file="ViewDeletedEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    /// <summary>
    /// Provides events args for the <see cref="IViewService.ViewDeleted"/> event.
    /// </summary>
    public class ViewDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewDeletedEventArgs"/> class.
        /// </summary>
        /// <param name="id">The id of the view that will be deleted.</param>
        public ViewDeletedEventArgs(string id)
        {
            this.ViewId = id;
        }

        /// <summary>
        /// Gets the id of the view that was deleted.
        /// </summary>
        public string ViewId { get; }
    }
}