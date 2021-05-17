// <copyright file="ViewModelNotResolvedException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;

    /// <summary>
    /// Provides a exception for when the views view model could not be resolved.
    /// </summary>
    public class ViewModelNotResolvedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotResolvedException"/> class.
        /// </summary>
        /// <param name="name">The name of the view model that could not be resolved.</param>
        public ViewModelNotResolvedException(string name)
            : base()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotResolvedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="name">The name of the view model that could not be resolved.</param>
        public ViewModelNotResolvedException(string message, string name)
            : base(message)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotResolvedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="name">The name of the view model that could not be resolved.</param>
        /// <param name="innerException">The inner exception to be included.</param>
        public ViewModelNotResolvedException(string name, Exception innerException)
            : base(string.Empty, innerException)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotResolvedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="name">The name of the view model that could not be resolved.</param>
        /// <param name="innerException">The inner exception to be included.</param>
        public ViewModelNotResolvedException(string message, string name, Exception innerException)
            : base(message, innerException)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the view model.
        /// </summary>
        public string Name { get; }
    }
}