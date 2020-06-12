// <copyright file="ShowDialogMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Wpf
{
    using System;
    using System.Linq;
    using System.Windows;

    public class ShowDialogMessage : IViewMessage
    {
        public string MessageName
        {
            get
            {
                return GenericMessageConstants.ShowDialog;
            }
        }

        public void SendMessage(IView view, ViewArguments args)
        {
            var wpfView = view as WpfView;
            if (wpfView == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var viewId = args.Get<string>(GenericMessageConstants.ViewId);
            if (string.IsNullOrWhiteSpace(viewId))
            {
                throw new ArgumentNullException($"Missing '{GenericMessageConstants.ViewId}' argument.");
            }

            var thisWindow = view.ViewReference as Window;
            var dialogView = wpfView.ViewService.Views.FirstOrDefault(x => x.Id.Equals(viewId, StringComparison.OrdinalIgnoreCase));
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
    }
}