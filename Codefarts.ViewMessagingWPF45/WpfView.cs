// <copyright file="WpfView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Windows;

    /// <summary>
    /// Provides a <see cref="IView"/> implementation for a Wpf UI element.
    /// </summary>
    /// <seealso cref="Codefarts.ViewMessaging.IView" />
    public class WpfView : IView
    {
        private FrameworkElement controlReference;
        private DataTemplate templateReference;

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

        public WpfView(IViewService viewService, FrameworkElement control, string viewName)
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
            if (control != null)
            {
                this.ViewReference = control;
                this.controlReference = control;
            }

            this.ViewName = viewName;
        }

        public WpfView(IViewService viewService, DataTemplate template, string viewName)
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
            if (template != null)
            {
                this.ViewReference = template;
                this.templateReference = template;
            }

            this.ViewName = viewName;
        }

        public WpfView(IViewService viewService, FrameworkElement control, string viewName, ViewArguments args)
            : this(viewService, control, viewName)
        {
            this.Arguments = args;
        }

        public WpfView(IViewService viewService, DataTemplate template, string viewName, ViewArguments args)
            : this(viewService, template, viewName)
        {
            this.Arguments = args;
        }
    }
}