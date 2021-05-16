// <copyright file="ConsoleView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    /// <summary>
    /// Provides a <see cref="IView"/> implementation for a Wpf UI element.
    /// </summary>
    /// <seealso cref="Codefarts.ViewMessaging.IView" />
    public class ConsoleView : IView
    {
        private IConsoleView viewReference;

        /// <inheritdoc />
        public string ViewName
        {
            get;
        }

        public ViewArguments Arguments
        {
            get; set;
        }

        /// <inheritdoc />
        public string Id
        {
            get;
        }

        public IViewService ViewService
        {
            get;
        }

        /// <inheritdoc />
        public object ViewReference
        {
            get;
        }

        public ConsoleView(IViewService viewService, IConsoleView viewRef, string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("No view name specified.", nameof(viewName));
            }

            this.ViewService = viewService ?? throw new ArgumentNullException(nameof(viewService));

            this.Id = Guid.NewGuid().ToString();
            if (viewRef != null)
            {
                this.ViewReference = viewRef;
            }

            this.ViewName = viewName;
        }

        public ConsoleView(IViewService viewService, IConsoleView viewRef, string viewName, ViewArguments args)
            : this(viewService, viewRef, viewName)
        {
            this.Arguments = args;
        }
    }
}