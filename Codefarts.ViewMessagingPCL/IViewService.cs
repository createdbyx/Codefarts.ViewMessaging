namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

    public interface IViewService
    {
        IEnumerable<IView> Views { get; }
        IView GetView(Guid id);
        bool DeleteView(Guid viewId);
        IView CreateView(string path);
    }
}
