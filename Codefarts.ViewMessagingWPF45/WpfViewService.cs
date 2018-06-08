namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    public class WpfViewService : IViewService, INotifyPropertyChanged
    {
        public IEnumerable<IView> Views
        {
            get { return this.viewReferences.Values; }
        }

        private IDictionary<Guid, IView> viewReferences = new Dictionary<Guid, IView>();

        public IView GetView(Guid id)
        {
            return this.viewReferences[id];
        }

        public bool DeleteView(Guid viewId)
        {
            return this.viewReferences.Remove(viewId);
        }

        public IView CreateView(string path)
        {
            // search through all assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.AsParallel().Where(this.FilterKnownLibraries);

            foreach (var asm in filteredAssemblies)
            {
                var types = asm.GetTypes().AsParallel();
                var name = path + "View";
                var views = types.Where(x => this.ViewTypeAndNameMatch(x, name));

                try
                {
                    var firstView = views.FirstOrDefault();
                    var item = firstView != null ? asm.CreateInstance(firstView.FullName) : null;
                    if (item != null)
                    {
                        var element = item as FrameworkElement;
                        var newView = new WpfView(this, element, path);
                        this.viewReferences.Add(newView.ViewId, newView);
                        return newView;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }

        private bool ViewTypeAndNameMatch(Type x, string name)
        {
            return x.Name.Equals(name) && x.IsInstanceOfType(typeof(FrameworkElement));
        }

        private bool FilterKnownLibraries(Assembly x)
        {
            return !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Microsoft");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
