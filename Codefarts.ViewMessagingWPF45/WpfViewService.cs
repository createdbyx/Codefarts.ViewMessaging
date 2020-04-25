using System.CodeDom;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfViewService"/> class.
        /// </summary>
        public WpfViewService()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += this.LoadFromSameFolder;
        }

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
            var useCache = args.Get("UseCache", true);
            var scanForAssemblies = args.Get("ScanAssemblies", true);
            var viewName = name + this.appendedViewName;
            var viewModelName = name + this.appendedViewModelName;

            WpfView wpfView;

            // attempt to create from cache first
            if (this.CreateViewFromCache(viewName, args, isDataTemplate, name, out wpfView))
            {
                this.ResolveViewModel(viewModelName, wpfView, scanForAssemblies, useCache);
                return wpfView;
            }

            // if not in cache scan for
            if (!this.ScanDomainForView(viewName, args, isDataTemplate, name, useCache, out wpfView))
            {
                if (scanForAssemblies && this.SearchForViewAssemblies(viewName, args, isDataTemplate, useCache, out wpfView))
                {
                    this.ResolveViewModel(viewModelName, wpfView, scanForAssemblies, useCache);
                    return wpfView;
                }
            }

            return null;
        }

        private void ResolveViewModel(string viewModelName, WpfView wpfView, bool scanForAssemblies, bool cacheViewModel)
        {
            if (!this.mVVMEnabled)
            {
                return;
            }

            if (viewModelName == null)
            {
                throw new ArgumentNullException(nameof(viewModelName));
            }

            if (wpfView == null)
            {
                throw new ArgumentNullException(nameof(wpfView));
            }

            var setContextCallback = new Action<WpfView, object>((view, viewModel) =>
             {
                 var dic = new Dictionary<string, object>(view.Arguments);
                 dic["ViewModel"] = viewModel;
                 view.Arguments = new ViewArguments(dic);
                 if (view.ViewReference is FrameworkElement element)
                 {
                     element.DataContext = viewModel;
                 }
             });

            // attempt to create from cache first
            object viewModelRef;
            if (this.CreateViewModelFromCache(viewModelName, wpfView, out viewModelRef))
            {
                setContextCallback(wpfView, viewModelRef);
                return;
            }

            // if not in cache scan for
            if (!this.ScanDomainForViewModel(viewModelName, cacheViewModel, out viewModelRef))
            {
                if (scanForAssemblies && this.SearchForViewModelAssemblies(viewModelName, cacheViewModel, out viewModelRef))
                {
                    setContextCallback(wpfView, viewModelRef);
                }
            }
        }

        private bool ScanDomainForViewModel(string viewName, bool cacheViewModel, out object viewModelRef)
        {
            // search through all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.AsParallel().Where(this.FilterKnownLibraries);

            foreach (var asm in filteredAssemblies)
            {
                if (this.GetViewModelFromAssembly(viewName, asm, cacheViewModel, out viewModelRef))
                {
                    return true;
                }
            }

            viewModelRef = null;
            return false;
        }

        private bool GetViewModelFromAssembly(string viewModelName, Assembly asm, bool cacheViewModel, out object viewModelRef)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            var types = asm.GetTypes().AsParallel();
            var viewModels = types.Where(x => x.IsClass && !x.IsAbstract && x != typeof(string) && x.Name.Equals(viewModelName, StringComparison.Ordinal));

            try
            {
                var firstViewModel = viewModels.FirstOrDefault();
                var item = firstViewModel != null ? asm.CreateInstance(firstViewModel.FullName) : null;

                if (item != null)
                {
                    // successfully created so add type to cache for faster access
                    if (cacheViewModel)
                    {
                        lock (previouslyCreatedViewModels)
                        {
                            previouslyCreatedViewModels[viewModelName] = firstViewModel;
                        }
                    }

                    viewModelRef = item;
                    return true;
                }
            }
            catch
            {
            }

            viewModelRef = null;
            return false;
        }

        private bool ScanDomainForView(string viewName, ViewArguments args, bool isDataTemplate, string name, bool cacheView, out WpfView wpfView)
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

        private bool CreateViewFromCache(string viewName, ViewArguments args, bool isDataTemplate, string dataTemplateName, out WpfView wpfView)
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

        private bool SearchForViewModelAssemblies(string viewName, bool cacheView, out object viewModelRef)
        {
            // ====== If we have made it here the view may be located in a currently unloaded assembly located in the app path

            // search application path assemblies
            // TODO: should use codebase? see https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // get all assemblies
            var viewModelFiles = Directory.GetFiles(appPath, "*.vmodels", SearchOption.AllDirectories);

            // check each file
            foreach (var file in viewModelFiles)
            {
                var asmFile = Path.ChangeExtension(file, ".dll");
                if (!File.Exists(asmFile))
                {
                    continue;
                }

                var assembly = Assembly.LoadFile(asmFile);
                if (this.GetViewModelFromAssembly(viewName, assembly, cacheView, out viewModelRef))
                {
                    return true;
                }
            }

            viewModelRef = null;
            return false;
        }

        private bool SearchForViewAssemblies(string viewName, ViewArguments args, bool isDataTemplate, bool cacheView, out WpfView wpfView)
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

                var assembly = Assembly.LoadFile(asmFile);
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
            var filter = new AssemblyName(args.Name);
            var fileMatches = Directory.GetFiles(folderPath, filter.Name + ".dll", SearchOption.AllDirectories);
            var assemblyPath = fileMatches.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
            {
                return null;
            }

            return Assembly.LoadFrom(assemblyPath);
        }

        private bool GetViewType(string name, ViewArguments args, Assembly asm, bool isDataTemplate, bool cacheView, out WpfView wpfView)
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
