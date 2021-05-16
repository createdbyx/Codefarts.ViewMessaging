// <copyright file="ShowWindowMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Console
{
    using System;
    using System.Windows;

    public class ShowMessage : IViewMessage
    {
        public string MessageName
        {
            get
            {
                return GenericMessageConstants.Show;
            }
        }

        public void SendMessage(IView view, ViewArguments args)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var ctrl = view.ViewReference as IConsoleView;
            if (ctrl == null)
            {
                throw new ArgumentException("ViewReference property is not a IConsoleView type.", nameof(view));
            }

            ctrl.Show();
        }
    }
}