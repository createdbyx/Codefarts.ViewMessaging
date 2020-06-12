// <copyright file="ViewCreatedEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    public class ViewCreatedEventArgs : EventArgs
    {
        public IView View
        {
            get; private set;
        }

        public ViewCreatedEventArgs(IView view)
        {
            this.View = view;
        }
    }
}