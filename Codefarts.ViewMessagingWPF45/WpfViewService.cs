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
        private static readonly Dictionary<string, Type> previouslyCreatedViews = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfViewService"/> class.
        /// </summary>
        public WpfViewService()
        {
            //  this.previouslyCreatedViews = new Dictionary<string, Type>();
        }

        public IEnumerable<IView> Views
        {
            get
            {
                return this.viewReferences.Values;
            }
        }

        private readonly IDictionary<string, IView> viewReferences = new Dictionary<string, IView>();
        private string appendedName = "View";

        public string AppendedName
        {
            get
            {
                return this.appendedName;
            }

            set
            {
                var currentValue = this.appendedName;
                if (currentValue != value)
                {
                    this.appendedName = value;
                    this.OnPropertyChanged(nameof(this.AppendedName));
                }
            }
        }

        public IView GetView(string id)
        {
            return this.viewReferences[id];
        }

        public bool DeleteView(string viewId)
        {
            return this.viewReferences.Remove(viewId);
        }

        public IView CreateView(string path)
        {
            if (previouslyCreatedViews.ContainsKey(path))
            {
                var firstView = previouslyCreatedViews[path];
                var item = firstView != null ? firstView.Assembly.CreateInstance(firstView.FullName) : null;
                if (item != null)
                {
                    var element = item as FrameworkElement;
                    var newView = new WpfView(this, element, path);
                    this.viewReferences.Add(newView.ViewId, newView);
                    return newView;
                }
            }

            // search through all assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.AsParallel().Where(this.FilterKnownLibraries);

            foreach (var asm in filteredAssemblies)
            {
                var types = asm.GetTypes().AsParallel();
                var name = path + this.appendedName;
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
                        // successfully created  so add type to cache for faster access
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[path] = firstView;
                        }

                        return newView;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private bool ViewTypeAndNameMatch(Type x, string name)
        {
            return x.Name.Equals(name) && x.IsSubclassOf(typeof(FrameworkElement));
        }

        private bool FilterKnownLibraries(Assembly x)
        {
            return !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Microsoft");
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Called when [property changed].</summary>
        /// <param name="propertyName">Name of the property.</param>
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
