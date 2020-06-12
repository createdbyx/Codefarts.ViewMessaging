// <copyright file="SetModelMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging.Wpf
{
    using System;
    using System.Windows;

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

            var ctrl = view.ViewReference as FrameworkElement;
            if (ctrl != null)
            {
                ctrl.DataContext = model;
                return;
            }

            var template = default(object); // this.templateReference;
            if (template != null)
            {
                // TODO: Not yet implemented and it may not need to be
                //throw new NotSupportedException("Setting a model on a DataTemplate is not yet supported. (And may never will be #SadFace)");
            }
        }
    }
}