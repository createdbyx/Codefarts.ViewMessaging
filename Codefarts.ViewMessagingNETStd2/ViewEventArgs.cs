// <copyright file="ViewEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    public class ViewEventArgs : EventArgs
    {
        public IView View
        {
            get; private set;
        }

        public ViewEventArgs(IView view)
        {
            this.View = view;
        }
    }
}