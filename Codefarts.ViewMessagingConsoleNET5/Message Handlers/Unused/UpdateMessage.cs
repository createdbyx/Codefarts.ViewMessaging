// <copyright file="UpdateMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Console
{
    using System;
    using System.Windows;
    using Codefarts.ViewMessaging;

    public class UpdateMessage : IViewMessage
    {
        public string MessageName
        {
            get
            {
                return GenericMessageConstants.Update;
            }
        }

        public void SendMessage(IView view, ViewArguments args)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var ctrl = view.ViewReference as UIElement;
            if (ctrl != null)
            {
                ctrl.UpdateLayout();
            }
        }
    }
}