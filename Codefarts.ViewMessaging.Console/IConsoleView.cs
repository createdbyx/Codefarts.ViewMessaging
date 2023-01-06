// <copyright file="IConsoleView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    public interface IConsoleView
    {
        public object DataContext
        {
            get; set;
        }

        void Show();
    }
}