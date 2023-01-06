// <copyright file="SetModelMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Console
{
    using System;

    public class SetModelMessage : IViewMessage
    {
        public string MessageName
        {
            get
            {
                return GenericMessageConstants.SetModel;
            }
        }

        public void SendMessage(IView view, ViewArguments args)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var model = args.Get<object>(GenericMessageConstants.SetModel);
            if (model == null)
            {
                throw new ArgumentNullException($"Missing '{GenericMessageConstants.SetModel}' argument.");
            }

            var ctrl = view.ViewReference as IConsoleView;
            if (ctrl != null)
            {
                ctrl.DataContext = model;
                return;
            }

            throw new InvalidCastException("ViewReference is not of type IConsoleView.");
        }
    }
}