// <copyright file="IViewService.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

#if NET20 // .NET 2.0 compatibility
    /// <summary>
    /// Provides a Func callback delegate for older .NET 2.0 compatibility.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
#endif

    /// <summary>
    /// Provides a interface for view services.
    /// </summary>
    public interface IViewService
    {
        /// <summary>
        /// Raised after a view is successfully created.
        /// </summary>
        event EventHandler<ViewEventArgs> ViewCreated;

        /// <summary>
        /// Raised after a view is successfully deleted.
        /// </summary>
        event EventHandler<ViewDeletedEventArgs> ViewDeleted;

        /// <summary>
        /// Raised before a view is deleted.
        /// </summary>
        event EventHandler<ViewEventArgs> BeforeViewDeleted;

        /// <summary>
        /// Gets the views that have been created.
        /// </summary>
        IEnumerable<IView> Views { get; }

        /// <summary>
        /// Gets the message handlers.
        /// </summary>
        /// <remarks>a message handler is a platform specific implementation of the <see cref="IViewMessage"/> interface that is designed to handle messages sent to a <see cref="IView"/> implementation.</remarks>
        IDictionary<string, IViewMessage> MessageHandlers { get; }

        /// <summary>
        /// Gets the view using a view id.
        /// </summary>
        /// <param name="id">The identifier that uniquely identifies the view.</param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView GetView(string id);

        /// <summary>
        /// Deletes a view using a view id.
        /// </summary>
        /// <param name="id">The identifier that uniquely identifies the view.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        bool DeleteView(string id);

        /// <summary>
        /// Creates a view from a name.
        /// </summary>
        /// <param name="viewName">The name that identifies the view to be created.</param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView CreateView(string viewName);

        /// <summary>
        /// Creates a view from a name.
        /// </summary>
        /// <param name="viewName">The name that identifies the view to be created.</param>
        /// <param name="args">Arguments to be passed to the view.</param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView CreateView(string viewName, ViewArguments args);

        /// <summary>
        /// Registers a view for quicker instanciation.
        /// </summary>
        /// <param name="viewName">The name of the view to register.</param>
        /// <param name="type">The type that is associated with the view.</param>
        void RegisterView(string viewName, Type type);

        /// <summary>
        /// Unregisters a view.
        /// </summary>
        /// <param name="viewName">The name of the view to unregister.</param>
        void UnregisterView(string viewName);
        
        /// <summary>
        /// Gets a dictionary of currently registered views.
        /// </summary>
        IDictionary<string, Type> RegisterdViews { get; }

        /// <summary>
        /// Registers a callback handler for creating a view.
        /// </summary>
        /// <param name="callback">The callback to be called.</param>
        /// <remarks>Before invoking internal view creation implementors should defer creation by calling each registered callback until
        /// a view is created. If no view was created fallback to internal view creation. This gives consumers ability to specify special
        /// case view creation for whatever platform they running on.</remarks>
        void RegisterHandler(Func<string, ViewArguments, IView> callback);

        /// <summary>
        /// Sends a the message to the view.
        /// </summary>
        /// <param name="message">The message identifier.</param>
        /// <param name="view">The view to send the message to.</param>
        /// <param name="args">The args to be passed to the message handler.</param>
        void SendMessage(string message, IView view, ViewArguments args);
    }
}