﻿// <copyright file="ConsoleViewService.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Codefarts.ViewMessaging.Console;

    /// <summary>
    /// Provides an implementation of <see cref="IViewService"/> for windows presentation foundation.
    /// </summary>
    public class ConsoleViewService : IViewService, INotifyPropertyChanged
    {
        /// <summary>
        /// The cache of previously created view types.
        /// </summary>
        /// <remarks>Only the type for the view is cached to prevent having to search thought the loaded assembly list to find it again.</remarks>
        private static readonly Dictionary<string, Type> previouslyCreatedViews = new Dictionary<string, Type>();

        private readonly IDictionary<string, IView> viewReferences = new Dictionary<string, IView>();
        private string appendedViewName = "View";
        private bool mVVMEnabled = false;
        private string appendedViewModelName = "ViewModel";
        private ViewModelResolver vmResolver;
        private IDictionary<string, IViewMessage> messageHandlers = new Dictionary<string, IViewMessage>();
        private List<Func<string, ViewArguments, IView>> handlerCallbacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleViewService"/> class.
        /// </summary>
        public ConsoleViewService()
        {
            this.handlerCallbacks = new List<Func<string, ViewArguments, IView>>();
            this.vmResolver = new ViewModelResolver();
            this.vmResolver.ViewModelTypeResolve += (s, e) => this.OnViewModelTypeResolve(e);
            // this.messageHandlers[GenericMessageConstants.ShowDialog] = new ShowDialogMessage();
            this.messageHandlers[GenericMessageConstants.Show] = new ShowMessage();
            this.messageHandlers[GenericMessageConstants.SetModel] = new SetModelMessage();
            // this.messageHandlers[GenericMessageConstants.Update] = new UpdateMessage();
            // this.messageHandlers[GenericMessageConstants.Refresh] = new RefreshMessage();
        }

        /// <summary>
        /// Occurs when a view model type needs to be resolved.
        /// </summary>
        public event ResolveEventHandler ViewModelTypeResolve;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler<ViewEventArgs> ViewCreated;

        /// <inheritdoc/>
        public event EventHandler<ViewDeletedEventArgs> ViewDeleted;

        /// <inheritdoc/>
        public event EventHandler<ViewEventArgs> BeforeViewDeleted;

        /// <summary>
        /// Gets or sets the string that is appended to the view model name.
        /// </summary>
        public string AppendedViewModelName
        {
            get { return this.appendedViewModelName; }

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

        /// <summary>
        /// Gets or sets weather MVVM style view creation should be used.
        /// </summary>
        public bool MvvmEnabled
        {
            get { return this.mVVMEnabled; }

            set
            {
                var currentValue = this.mVVMEnabled;
                if (currentValue != value)
                {
                    this.mVVMEnabled = value;
                    this.OnPropertyChanged(nameof(this.MvvmEnabled));
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IView> Views
        {
            get { return this.viewReferences.Values; }
        }

        public string AppendedViewName
        {
            get { return this.appendedViewName; }

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

        /// <inheritdoc/>
        public IView GetView(string id)
        {
            return this.viewReferences[id];
        }

        /// <inheritdoc/>
        public bool DeleteView(string id)
        {
            return this.viewReferences.Remove(id);
        }

        /// <inheritdoc/>
        public IView CreateView(string name)
        {
            return CreateView(name, new ViewArguments());
        }

        /// <inheritdoc/>
        public IView CreateView(string name, ViewArguments args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var useCache = args.Get("UseCache", true);
            var scanForAssemblies = args.Get("ScanAssemblies", true);
            var viewName = name + this.appendedViewName;
            var viewModelName = name + this.appendedViewModelName;

            IView consoleView;

            // try to use handlers first
            foreach (var callback in this.handlerCallbacks)
            {
                consoleView = callback(name, args);
                if (consoleView != null)
                {
                    this.OnViewCreated(consoleView);
                    return consoleView;
                }
            }

            // attempt to create from cache first
            if (this.CreateViewFromCache(viewName, args, name, out consoleView))
            {
                this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, consoleView);
                return consoleView;
            }

            // if not in cache scan for
            if (this.ScanDomainForView(viewName, args, name, useCache, out consoleView))
            {
                this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, consoleView);
                return consoleView;
            }

            if (scanForAssemblies && this.SearchForViewAssemblies(viewName, args, name, useCache, out consoleView))
            {
                this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, consoleView);
                return consoleView;
            }

            return null;
        }

        public void SetViewModelAndReturn(ViewArguments args, string viewModelName, bool scanForAssemblies, bool useCache, IView wpfView)
        {
            var model = args.Get<object>(GenericMessageConstants.SetModel, null);
            if (model == null)
            {
                this.TryToResolveViewModel(viewModelName, scanForAssemblies, useCache, wpfView);
                this.OnViewCreated(wpfView);
            }
            else
            {
                this.SendMessage(GenericMessageConstants.SetModel, wpfView, args);
                this.OnViewCreated(wpfView);
            }
        }

        public void RegisterHandler(Func<string, ViewArguments, IView> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.handlerCallbacks.Add(callback);
        }

        public IDictionary<string, IViewMessage> MessageHandlers
        {
            get { return this.messageHandlers; }
        }

        public void SendMessage(string message, IView view, ViewArguments args)
        {
            this.MessageHandlers[message].SendMessage(view, args);
        }

        private void TryToResolveViewModel(string viewModelName, bool scanForAssemblies, bool useCache, IView wpfView)
        {
            if (this.mVVMEnabled)
            {
                this.vmResolver.ResolveViewModel(this, viewModelName, wpfView, scanForAssemblies, useCache);
            }
        }

        private bool ScanDomainForView(string viewName, ViewArguments args, string name, bool cacheView, out IView wpfView)
        {
            wpfView = null;
            //if (isDataTemplate)
            //{
            //    var item = Application.Current.TryFindResource(viewName);
            //    if (item != null)
            //    {
            //        var element = item as DataTemplate;
            //        var newView = new ConsoleView(this, element, name, args == null ? null : new ViewArguments(args));
            //        this.viewReferences.Add(newView.Id, newView);

            //        // successfully created  so add type to cache for faster access
            //        if (cacheView)
            //        {
            //            lock (previouslyCreatedViews)
            //            {
            //                previouslyCreatedViews[viewName] = element.DataType as Type;
            //            }
            //        }

            //        return true;
            //    }
            //}
            //else
            //{
            var filter = new Func<Assembly, bool>(x => !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Microsoft"));

            // search through all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.AsParallel().Where(filter);

            foreach (var asm in filteredAssemblies)
            {
                if (this.GetViewType(viewName, args, asm, name, cacheView, out wpfView))
                {
                    return true;
                }
            }
            // }

            return false;
        }

        private bool CreateViewFromCache(string viewName, ViewArguments args, string dataTemplateName, out IView wpfView)
        {
            wpfView = null;

            if (previouslyCreatedViews.ContainsKey(viewName))
            {
                var firstView = previouslyCreatedViews[viewName];
                //if (isDataTemplate)
                //{
                //    var item = firstView != null ? Application.Current.TryFindResource(viewName) : null;
                //    if (item != null)
                //    {
                //        var element = item as DataTemplate;
                //        var newView = new ConsoleView(this, element, dataTemplateName, args == null ? null : new ViewArguments(args));
                //        this.viewReferences.Add(newView.Id, newView);
                //        {
                //            wpfView = newView;
                //            return true;
                //        }
                //    }
                //}
                //else
                //{
                var viewRef = firstView != null ? firstView.Assembly.CreateInstance(firstView.FullName) : null;
                if (viewRef != null)
                {
                    var consoleView = viewRef as IConsoleView;
                    var newView = new ConsoleView(this, consoleView, dataTemplateName, args == null ? null : new ViewArguments(args));
                    this.viewReferences.Add(newView.Id, newView);
                    {
                        wpfView = newView;
                        return true;
                    }
                }
                //}
            }

            return false;
        }

        private bool SearchForViewAssemblies(string viewName, ViewArguments args, string name, bool cacheView, out IView wpfView)
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

#if NETCOREAPP3_1
                var asmName = new AssemblyName(Path.GetFileNameWithoutExtension(asmFile));
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(asmName);
#else
                var assembly = Assembly.LoadFile(asmFile);
#endif
                //if (isDataTemplate)
                //{
                //    var resourceNames = assembly.GetManifestResourceNames();
                //    var res = resourceNames.FirstOrDefault(x => x.EndsWith(name + "DataTemplate.xaml", StringComparison.OrdinalIgnoreCase));
                //    if (res != null)
                //    {
                //        using (var stream = assembly.GetManifestResourceStream(res))
                //        {
                //            var context = new ParserContext();
                //            var resource = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream, context);
                //            Application.Current.Resources.MergedDictionaries.Add(resource);
                //        }
                //    }
                //}

                if (this.GetViewType(viewName, args, assembly, name, cacheView, out wpfView))
                {
                    return true;
                }
            }

            wpfView = null;
            return false;
        }

        private bool GetViewType(string viewName, ViewArguments args, Assembly asm, string name, bool cacheView, out IView wpfView)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            //if (isDataTemplate)
            //{
            //    var item = Application.Current.TryFindResource(viewName);
            //    if (item != null)
            //    {
            //        var element = item as DataTemplate;
            //        var newView = new ConsoleView(this, element, name, args == null ? null : new ViewArguments(args));
            //        this.viewReferences.Add(newView.Id, newView);

            //        // successfully created so add type to cache for faster access
            //        if (cacheView)
            //        {
            //            lock (previouslyCreatedViews)
            //            {
            //                previouslyCreatedViews[viewName] = item.GetType();
            //            }
            //        }

            //        wpfView = newView;
            //        return true;
            //    }
            //}

            var types = asm.GetTypes().AsParallel();
            var views = types.Where(x => this.ViewTypeAndNameMatch(x, viewName));

            try
            {
                var firstView = views.FirstOrDefault();
                var viewRef = firstView != null ? asm.CreateInstance(firstView.FullName) : null;

                if (viewRef != null)
                {
                    var consoleView = viewRef as IConsoleView;
                    var newView = new ConsoleView(this, consoleView, name, args == null ? null : new ViewArguments(args));
                    this.viewReferences.Add(newView.Id, newView);

                    // successfully created so add type to cache for faster access
                    if (cacheView)
                    {
                        lock (previouslyCreatedViews)
                        {
                            previouslyCreatedViews[viewName] = firstView;
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

        private bool ViewTypeAndNameMatch(Type x, string name)
        {
            if (x.Name.Equals(name))
            {
                //if (isDataTemplate)
                //{
                //    return x.IsSubclassOf(typeof(DataTemplate));
                //}

                return x.IsAssignableTo(typeof(IConsoleView));
            }

            return false;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Called after a view if successfully created.
        /// </summary>
        /// <param name="view">Reference to the view that was created.</param>
        protected virtual void OnViewCreated(IView view)
        {
            var handler = this.ViewCreated;
            if (handler != null)
            {
                handler(this, new ViewEventArgs(view));
            }
        }

        /// <summary>
        /// Raises the <see cref="ViewModelTypeResolve"/> event and returns the result.
        /// </summary>
        /// <param name="args">The type creation args containing information about the type to create.</param>
        /// <returns>An object reference that was create from the type information.</returns>
        /// <remarks>If no event handlers are available will return null.</remarks>
        protected virtual object OnViewModelTypeResolve(ResolveEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var handler = this.ViewModelTypeResolve;
            if (handler != null)
            {
                return handler(this, args);
            }

            return null;
        }
    }
}