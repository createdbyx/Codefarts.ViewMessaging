// <copyright file="RefreshMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Wpf
{
    using System;
    using System.Windows.Threading;

    public class RefreshMessage : IViewMessage
    {
        public string MessageName
        {
            get
            {
                return GenericMessageConstants.Refresh;
            }
        }

        public void SendMessage(IView view, ViewArguments args)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var ctrl = view.ViewReference as DispatcherObject;
            if (ctrl != null)
            {
                ctrl.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
            }
        }
    }
}