namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public class WpfView : IView
    {
        private readonly string viewPath;
        private FrameworkElement controlReference;

        public string ViewPath
        {
            get { return this.viewPath; }
        }

        public Guid ViewId { get; }

        public WpfViewService ViewService { get; }

        public object ViewReference { get; }

        public WpfView(WpfViewService viewService, FrameworkElement control, string path)
        {
            this.ViewId = Guid.NewGuid();
            this.ViewReference = control;
            this.controlReference = control;
            this.ViewService = viewService;
            this.viewPath = path;
        }

        public void SendMessage(IDictionary<string, string> args)
        {
            foreach (var pair in args)
            {
                switch (pair.Key.ToLowerInvariant())
                {
                    case "show":
                        var ctrl = this.controlReference as Window;
                        if (ctrl != null)
                        {
                            ctrl.Show();
                        }

                        break;

                    case "showdialog":
                        if(pair.va)
                        var thisWindow = this.controlReference as Window;
                        var ctrl = this.controlReference as Window;
                        if (ctrl == null)
                        {
                            ctrl.Show();
                        }

                        break;
                }
            }
        }
    }
}