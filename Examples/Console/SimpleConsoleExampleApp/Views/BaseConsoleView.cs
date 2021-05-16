// <copyright file="BaseConsoleView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace SimpleConsoleExampleApp
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Codefarts.ViewMessaging;

    public abstract class BaseConsoleView<T> : IConsoleView, INotifyPropertyChanged
        where T : class
    {
        private object dataContext;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public object DataContext
        {
            get
            {
                return this.dataContext;
            }

            set
            {
                var currentValue = this.dataContext;
                if (currentValue != value)
                {
                    if (value is not T)
                    {
                        throw new InvalidCastException($"DataContext must be of type {typeof(T).FullName}.");
                    }

                    this.dataContext = value;
                    this.OnPropertyChanged(nameof(this.DataContext));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Show()
        {
            TryAgain:
            this.Draw();

            // normally user input would be handled by a presentation implementation like wpf for example, but because we are using console output
            // and this class is responsible for drawing the view we write the user input logic here. More complex systems would separate concerns
            // better but this code is just for example purposes to get something on screen.
            this.HandleInput();

            goto TryAgain;
        }

        public abstract void HandleInput();

        private void Draw()
        {
            Console.Clear();
            this.DrawMenu();
        }

        public abstract void DrawMenu();
    }
}