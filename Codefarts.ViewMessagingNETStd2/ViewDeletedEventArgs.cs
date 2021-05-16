// <copyright file="ViewDeletedEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    public class ViewDeletedEventArgs : EventArgs
    {
        public string ViewId
        {
            get; private set;
        }

        public ViewDeletedEventArgs(string id)
        {
            this.ViewId = id;
        }
    }
}