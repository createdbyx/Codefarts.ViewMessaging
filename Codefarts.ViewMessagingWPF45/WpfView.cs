namespace Codefarts.ViewMessaging
{
    using System;
    using System.Linq;
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
            get; private set;
        }

        /// <inheritdoc />
        public string Id
        {
            get;
        }

        public WpfViewService ViewService
        {
            get;
        }

        /// <inheritdoc />
        public object ViewReference
        {
            get;
        }

        public WpfView(WpfViewService viewService, FrameworkElement control, string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException(nameof(viewName));
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

        public WpfView(WpfViewService viewService, DataTemplate template, string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("Missing view name.", nameof(viewName));
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

        public WpfView(WpfViewService viewService, FrameworkElement control, string viewName, ViewArguments args)
            : this(viewService, control, viewName)
        {
            this.Arguments = args;
        }

        public WpfView(WpfViewService viewService, DataTemplate template, string viewName, ViewArguments args)
            : this(viewService, template, viewName)
        {
            this.Arguments = args;
        }

        /// <inheritdoc />
        public virtual void SendMessage(ViewArguments args)
        {
            foreach (var pair in args)
            {
                switch (pair.Key)
                {
                    case GenericMessageConstants.Show:
                        this.DoShowWindow();
                        break;

                    case GenericMessageConstants.ShowDialog:
                        this.DoShowDialog(pair.Value.ToString());
                        break;

                    case GenericMessageConstants.SetModel:
                        this.DoSetModel(pair.Value);
                        break;
                }
            }
        }

        protected virtual void DoSetModel(object model)
        {
            var ctrl = this.controlReference;
            if (ctrl != null)
            {
                ctrl.DataContext = model;
            }
        }

        protected virtual void DoShowDialog(string viewId)
        {
            if (string.IsNullOrWhiteSpace(viewId))
            {
                throw new ArgumentNullException(GenericMessageConstants.ShowDialog);
            }

            var ctrl = this.controlReference;
            if (ctrl == null)
                return;

            var thisWindow = this.controlReference as Window;
            var dialogView = this.ViewService.Views.FirstOrDefault(x => x.Id.Equals(viewId, StringComparison.OrdinalIgnoreCase));
            var dialogWindow = dialogView.ViewReference as Window;
            if (thisWindow == null)
            {
                throw new NullReferenceException("Parent view not a window.");
            }

            if (dialogWindow == null)
            {
                throw new NullReferenceException("Dialog view not a window.");
            }


            dialogWindow.Owner = thisWindow;
            dialogWindow.ShowDialog();
        }

        protected virtual void DoShowWindow()
        {
            // ReSharper disable once UsePatternMatching
            var ctrl = this.controlReference as Window;
            if (ctrl != null)
            {
                ctrl.Show();
            }
        }
    }
}