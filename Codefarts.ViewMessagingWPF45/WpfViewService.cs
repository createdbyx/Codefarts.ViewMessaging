using System.IO;

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    public class WpfViewService : IViewService, INotifyPropertyChanged
    {
        /// <summary>
        /// The cache of previously created view types.
        /// </summary>
        /// <remarks>Only the type for the view is cached to prevent having to search thought the loaded assembly list to find it again.</remarks>
        private static readonly Dictionary<string, Type> previouslyCreatedViews = new Dictionary<string, Type>();
        private readonly IDictionary<string, IView> viewReferences = new Dictionary<string, IView>();
        private string appendedName = "View";
        public IEnumerable<IView> Views
        {
            get
            {
                return this.viewReferences.Values;
            }
        }

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
            return CreateView(path, null);
        }

        private T GetArgumentValue<T>(IDictionary<string, object> dictionary, string key)
        {
            var defaultValue = default(T);
            object value;
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                defaultValue = (T)value;
            }

            return defaultValue;
        }

        public IView CreateView(string path, IDictionary<string, object> args)
        {
            var isDataTemplate = this.GetArgumentValue<bool>(args, "IsDataTemplate");
            var cacheView = this.GetArgumentValue<bool>(args, "CacheView");
            var scanForAssemblies = this.GetArgumentValue<bool>(args, "ScanAssemblies");
            var name = path + this.appendedName;

            if (previouslyCreatedViews.ContainsKey(path))
            {
                var firstView = previouslyCreatedViews[path];
                if (isDataTemplate)
                {
                    var item = firstView != null ? Application.Current.TryFindResource(name) : null;
                    if (item != null)
                    {
                        var element = item as DataTemplate;
                        var newView = new WpfView(this, element, path, args == null ? null : new ReadOnlyDictionary<string, object>(args));
                        this.viewReferences.Add(newView.ViewId, newView);
                        return newView;
                    }
                }
                else
                {
                    var item = firstView != null ? firstView.Assembly.CreateInstance(firstView.FullName) : null;
                    if (item != null)
                    {
                        var element = item as FrameworkElement;
                        var newView = new WpfView(this, element, path, args == null ? null : new ReadOnlyDictionary<string, object>(args));
                        this.viewReferences.Add(newView.ViewId, newView);
                        return newView;
                    }
                }
            }

            if (isDataTemplate)
            {
                var item = Application.Current.TryFindResource(name);
                if (item != null)
                {
                    var element = item as DataTemplate;
                    var newView = new WpfView(this, element, path, args == null ? null : new ReadOnlyDictionary<string, object>(args));
                    this.viewReferences.Add(newView.ViewId, newView);

                    // successfully created  so add type to cache for faster access
                    if (cacheView)
                    {
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[path] = element.DataType as Type;
                        }
                    }

                    return newView;
                }
            }
            else
            {
                // search through all loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var filteredAssemblies = assemblies.AsParallel().Where(this.FilterKnownLibraries);

                foreach (var asm in filteredAssemblies)
                {
                    if (this.GetViewType(path, args, asm, name, isDataTemplate, cacheView, out var wpfView))
                    {
                        return wpfView;
                    }
                }
            }

            // ====== If we have made it here the view may be located in a currently unloaded assembly located in the app path

            // search application path assemblies
            // TODO: should use codebase? see https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // get all assemblies
            var viewFiles = Directory.GetFiles(appPath, "*.cviews", SearchOption.AllDirectories);

            // check each file
            foreach (var file in viewFiles)
            {
                var asmFile = Path.ChangeExtension(file, ".dll");
                if (!File.Exists(asmFile))
                {
                    continue;
                }

                Assembly assembly = null;
                try
                {
                    var ad = AppDomain.CreateDomain("TempViewMessagingDomain");
                    var symbolFile = Path.ChangeExtension(asmFile, ".pdb");
                    var rawAssembly = File.ReadAllBytes(asmFile);
                    var rawSymbolStore = File.Exists(symbolFile) ? File.ReadAllBytes(symbolFile) : null;
                    assembly = rawSymbolStore == null ? ad.Load(rawAssembly) : ad.Load(rawAssembly, rawSymbolStore);
                    AppDomain.Unload(ad);
                }
                catch
                {
                    continue;
                }

                if (this.GetViewType(path, args, assembly, name, isDataTemplate, cacheView, out var wpfView))
                {
                    return wpfView;
                }
            }

            return null;
        }

        private bool GetViewType(string path, IDictionary<string, object> args, Assembly asm, string name, bool isDataTemplate, bool cacheView, out IView wpfView)
        {
            var types = asm.GetTypes().AsParallel();
            var views = types.Where(x => this.ViewTypeAndNameMatch(x, name, isDataTemplate));

            try
            {
                var firstView = views.FirstOrDefault();
                var item = firstView != null ? asm.CreateInstance(firstView.FullName) : null;

                if (item != null)
                {
                    var element = item as FrameworkElement;
                    var newView = new WpfView(this, element, path, args == null ? null : new ReadOnlyDictionary<string, object>(args));
                    this.viewReferences.Add(newView.ViewId, newView);

                    // successfully created so add type to cache for faster access
                    if (cacheView)
                    {
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[path] = firstView;
                        }
                    }

                    wpfView = newView;
                    return true;
                }
            }
            catch
            {
            }

            wpfView = null;
            return false;
        }

        private bool ViewTypeAndNameMatch(Type x, string name, bool isDataTemplate)
        {
            if (x.Name.Equals(name))
            {
                if (isDataTemplate)
                {
                    return x.IsSubclassOf(typeof(DataTemplate));
                }

                return x.IsSubclassOf(typeof(FrameworkElement));
            }

            return false;
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
