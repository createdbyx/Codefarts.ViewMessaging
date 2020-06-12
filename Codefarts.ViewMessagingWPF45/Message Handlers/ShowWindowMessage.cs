// <copyright file="ShowWindowMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Wpf
{
    using System;
    using System.Windows;

    public class ShowWindowMessage : IViewMessage
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

            var ctrl = view.ViewReference as Window;
            if (ctrl == null)
            {
                throw new ArgumentException("ViewReference property is not a wpf Window type.", nameof(view));
            }

            ctrl.Show();
        }
    }
}