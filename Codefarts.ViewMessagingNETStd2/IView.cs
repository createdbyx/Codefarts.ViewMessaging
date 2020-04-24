namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides a interface that represents a UI element.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        string ViewName
        {
            get;
        }

        /// <summary>
        /// Gets the unique identifier for the view.
        /// </summary>
        /// <remarks>Each view reference has a unique identifier.</remarks>
        string Id
        {
            get;
        }

        /// <summary>
        /// Gets the reference to the view object.
        /// </summary>
        object ViewReference
        {
            get;
        }

        /// <summary>
        /// Sends a message to the view.
        /// </summary>
        /// <param name="args">The named arguments to be sent.</param>
        void SendMessage(ViewArguments args);
    }
}