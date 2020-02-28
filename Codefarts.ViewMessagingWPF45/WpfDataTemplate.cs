﻿namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public class WpfDataTemplate : IView
    {
        private readonly string viewPath;
        private DataTemplate templateReference;

        public string ViewPath
        {
            get
            {
                return this.viewPath;
            }
        }

        public string ViewId { get; }

        public WpfViewService ViewService { get; }

        public object ViewReference { get; }

        public WpfDataTemplate(WpfViewService viewService, DataTemplate template, string path)
        {
            this.ViewId = Guid.NewGuid().ToString("N");
            this.ViewReference = template;
            this.templateReference = template;
            this.ViewService = viewService;
            this.viewPath = path;
        }

        public void SendMessage(IDictionary<string, object> args)
        {
            // does nothing

            //foreach (var pair in args)
            //{
            //    switch (pair.Key)
            //    {
            //        case GenericMessageConstants.Show:
            //            this.DoShowWindow();
            //            break;

            //        case GenericMessageConstants.ShowDialog:
            //            this.DoShowDialog(pair.Value.ToString());
            //            break;

            //        case GenericMessageConstants.SetModel:
            //            this.DoSetModel(pair.Value);
            //            break;
            //    }
            //}
        }

        /*
        private void DoSetModel(object model)
        {
            var ctrl = this.templateReference;
            if (ctrl != null)
            {
                ctrl.DataContext = model;
            }
        }

        private void DoShowDialog(string viewId)
        {
            if (string.IsNullOrWhiteSpace(viewId))
            {
                throw new ArgumentNullException(GenericMessageConstants.ShowDialog);
            }

            var thisWindow = this.templateReference as Window;
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
            var ctrl = this.templateReference as Window;
            if (ctrl != null)
            {
                ctrl.Show();
            }
        }
        */
    }
}