namespace Codefarts.ViewMessaging
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a interface for view services.
    /// </summary>
    public interface IViewService
    {
        /// <summary>
        /// Gets the views that have been created.
        /// </summary>   
        IEnumerable<IView> Views
        {
            get;
        }

        /// <summary>
        /// Gets the view using a view id.
        /// </summary>
        /// <param name="id">The identifier that uniquely identifies the view.</param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView GetView(string id);

        /// <summary>
        /// Deletes a view using a view id.
        /// </summary>
        /// <param name="id">The identifier that uniquely identifies the view.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        bool DeleteView(string id);

        /// <summary>
        /// Creates a view from a name.
        /// </summary>
        /// <param name="viewName">The name that identifies the view to be created.</param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView CreateView(string viewName);

        /// <summary>
        /// Creates a view from a name.
        /// </summary>
        /// <param name="viewName">The name that identifies the view to be created.</param>
        /// <param name="args"></param>
        /// <returns>A implementation of a <see cref="IView"/> interface.</returns>
        IView CreateView(string viewName, ViewArguments args);
    }
}
