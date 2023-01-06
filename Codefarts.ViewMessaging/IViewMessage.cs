// <copyright file="IViewMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides a interface for implementing view messages.
    /// </summary>
    public interface IViewMessage
    {
        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        string MessageName
        {
            get;
        }

        /// <summary>
        /// Sends this message instance to the desired view.
        /// </summary>
        /// <param name="view">A reference to the view that we want to send this message to.</param>
        /// <param name="args">Any arguments that may be relevant for this message to operate.</param>
        void SendMessage(IView view, ViewArguments args);
    }
}