using System.Linq;

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

        public string ViewId { get; }

        public WpfViewService ViewService { get; }

        public object ViewReference { get; }

        public WpfView(WpfViewService viewService, FrameworkElement control, string path)
        {
            this.ViewId = Guid.NewGuid().ToString("N");
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
                        this.DoShowWindow();

                        break;

                    case "showdialog":
                        this.DoShowDialog(pair.Value);
                        break;
                }
            }
        }

        private void DoShowDialog(string viewId)
        {
            if (string.IsNullOrWhiteSpace(viewId))
            {
                throw new ArgumentNullException("showdialog");
            }

            var thisWindow = this.controlReference as Window;
            var dialogView = this.ViewService.Views.FirstOrDefault(x => x.ViewId.Equals(viewId, StringComparison.OrdinalIgnoreCase));
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

        private void DoShowWindow()
        {
            var ctrl = this.controlReference as Window;
            if (ctrl != null)
            {
                ctrl.Show();
            }
        }
    }
}