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
        private static readonly Dictionary<string, Type> previouslyCreatedViewModels = new Dictionary<string, Type>();
        private readonly IDictionary<string, IView> viewReferences = new Dictionary<string, IView>();
        private string appendedViewName = "View";
        private bool mVVMEnabled = false;
        private string appendedViewModelName = "ViewModel";

        public string AppendedViewModelName
        {
            get
            {
                return this.appendedViewModelName;
            }

            set
            {
                var currentValue = this.appendedViewModelName;
                if (currentValue != value)
                {
                    this.appendedViewModelName = value;
                    this.OnPropertyChanged(nameof(this.AppendedViewModelName));
                }
            }
        }

        public bool MVVMEnabled
        {
            get
            {
                return this.mVVMEnabled;
            }

            set
            {
                var currentValue = this.mVVMEnabled;
                if (currentValue != value)
                {
                    this.mVVMEnabled = value;
                    this.OnPropertyChanged(nameof(this.MVVMEnabled));
                }
            }
        }

        public IEnumerable<IView> Views
        {
            get
            {
                return this.viewReferences.Values;
            }
        }

        public string AppendedViewName
        {
            get
            {
                return this.appendedViewName;
            }

            set
            {
                var currentValue = this.appendedViewName;
                if (currentValue != value)
                {
                    this.appendedViewName = value;
                    this.OnPropertyChanged(nameof(this.AppendedViewName));
                }
            }
        }

        public IView GetView(string id)
        {
            return this.viewReferences[id];
        }

        public bool DeleteView(string id)
        {
            return this.viewReferences.Remove(id);
        }

        public IView CreateView(string name)
        {
            return CreateView(name, new ViewArguments());
        }

        public IView CreateView(string name, ViewArguments args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var isDataTemplate = args.Get<bool>("IsDataTemplate");
            var cacheView = args.Get("CacheView", true);
            var scanForAssemblies = args.Get("ScanAssemblies", true);
            var viewName = name + this.appendedViewName;
            var viewModelName = name + this.appendedViewModelName;

            IView wpfView;

            // attempt to create from cache first
            if (this.CreateFromCache(viewName, args, isDataTemplate, name, out wpfView))
            {
                this.ResolveViewModel(viewModelName, wpfView);
                return wpfView;
            }

            // if not in cache scan for
            if (!this.ScanDomainForView(viewName, args, isDataTemplate, name, cacheView, out wpfView))
            {
                if (scanForAssemblies && this.SearchAppFolderForAssemblies(viewName, args, name, isDataTemplate, cacheView, out wpfView))
                {
                    this.ResolveViewModel(viewModelName, wpfView);
                    return wpfView;
                }
            }

            return null;
        }

        private void ResolveViewModel(string viewModelName, IView wpfView)
        {
            if (viewModelName == null)
            {
                throw new ArgumentNullException(nameof(viewModelName));
            }

            if (wpfView == null)
            {
                throw new ArgumentNullException(nameof(wpfView));
            }

            if (!this.mVVMEnabled)
            {
                return;
            }

            // wpfView.
        }

        private bool ScanDomainForView(string viewName, ViewArguments args, bool isDataTemplate, string name, bool cacheView, out IView wpfView)
        {
            wpfView = null;
            if (isDataTemplate)
            {
                var item = Application.Current.TryFindResource(name);
                if (item != null)
                {
                    var element = item as DataTemplate;
                    var newView = new WpfView(this, element, viewName, args == null ? null : new ViewArguments(args));
                    this.viewReferences.Add(newView.Id, newView);

                    // successfully created  so add type to cache for faster access
                    if (cacheView)
                    {
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[viewName] = element.DataType as Type;
                        }
                    }

                    return true;
                }
            }
            else
            {
                // search through all loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var filteredAssemblies = assemblies.AsParallel().Where(this.FilterKnownLibraries);

                foreach (var asm in filteredAssemblies)
                {
                    if (this.GetViewType(viewName, args, asm, isDataTemplate, cacheView, out wpfView))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CreateViewModelFromCache(string name, IView wpfView, out object viewModelRef)
        {
            if (previouslyCreatedViewModels.ContainsKey(name))
            {
                var viewModel = previouslyCreatedViewModels[name];
                viewModelRef = viewModel != null ? viewModel.Assembly.CreateInstance(viewModel.FullName) : null;
                if (wpfView.ViewReference is FrameworkElement)
                {
                    ((FrameworkElement)wpfView.ViewReference).DataContext = viewModelRef;
                }
                else if (wpfView.ViewReference is DataTemplate)
                {
                    throw new NotImplementedException();
                }

                return true;
            }

            viewModelRef = null;
            return false;
        }

        private bool CreateFromCache(string viewName, ViewArguments args, bool isDataTemplate, string dataTemplateName, out IView wpfView)
        {
            wpfView = null;

            if (previouslyCreatedViews.ContainsKey(viewName))
            {
                var firstView = previouslyCreatedViews[viewName];
                if (isDataTemplate)
                {
                    var item = firstView != null ? Application.Current.TryFindResource(dataTemplateName) : null;
                    if (item != null)
                    {
                        var element = item as DataTemplate;
                        var newView = new WpfView(this, element, viewName, args == null ? null : new ViewArguments(args));
                        this.viewReferences.Add(newView.Id, newView);
                        {
                            wpfView = newView;
                            return true;
                        }
                    }
                }
                else
                {
                    var item = firstView != null ? firstView.Assembly.CreateInstance(firstView.FullName) : null;
                    if (item != null)
                    {
                        var element = item as FrameworkElement;
                        var newView = new WpfView(this, element, viewName, args == null ? null : new ViewArguments(args));
                        this.viewReferences.Add(newView.Id, newView);
                        {
                            wpfView = newView;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool SearchAppFolderForAssemblies(string viewName, ViewArguments args, string name, bool isDataTemplate, bool cacheView, out IView wpfView)
        {
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
                    var currentDomain = AppDomain.CurrentDomain;
                    currentDomain.AssemblyResolve += this.LoadFromSameFolder;

                    assembly = Assembly.LoadFile(asmFile);

                    currentDomain.AssemblyResolve -= this.LoadFromSameFolder;
                }
                catch
                {
                    continue;
                }

                if (this.GetViewType(viewName, args, assembly, isDataTemplate, cacheView, out wpfView))
                {
                    return true;
                }
            }

            wpfView = null;
            return false;
        }

        private Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var allFiles = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);
            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private bool GetViewType(string name, ViewArguments args, Assembly asm, bool isDataTemplate, bool cacheView, out IView wpfView)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            var types = asm.GetTypes().AsParallel();
            var views = types.Where(x => this.ViewTypeAndNameMatch(x, name, isDataTemplate));

            try
            {
                var firstView = views.FirstOrDefault();
                var item = firstView != null ? asm.CreateInstance(firstView.FullName) : null;

                if (item != null)
                {
                    var element = item as FrameworkElement;
                    var newView = new WpfView(this, element, name, args == null ? null : new ViewArguments(args));
                    this.viewReferences.Add(newView.Id, newView);

                    // successfully created so add type to cache for faster access
                    if (cacheView)
                    {
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[name] = firstView;
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
