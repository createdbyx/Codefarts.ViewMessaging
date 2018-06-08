namespace Codefarts.ViewMessaging
{
    using System.Collections.Generic;

    public interface IViewService
    {
        IEnumerable<IView> Views { get; }
        IView GetView(string id);
        bool DeleteView(string viewId);
        IView CreateView(string path);
    }
}
