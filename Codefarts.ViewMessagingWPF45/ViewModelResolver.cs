// <copyright file="WpfViewService.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// A view model resolver class.
    /// </summary>
    /// <remarks>Used by <see cref="WpfViewService"/> for resolving view model classes.</remarks>
    internal class ViewModelResolver
    {
        private static readonly Dictionary<string, Type> previouslyCreatedViewModels = new Dictionary<string, Type>();

        /// <summary>
        /// Occurs when a view model type needs to be resolved.
        /// </summary>
        public event ResolveEventHandler ViewModelTypeResolve;

        internal void ResolveViewModel(string viewModelName, WpfView wpfView, bool scanForAssemblies, bool cacheViewModel)
        {
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

                 if (view.ViewReference is FrameworkElement)
                 {
                     ((FrameworkElement)view.ViewReference).DataContext = viewModel;
                 }
                 else if (view.ViewReference is DataTemplate)
                 {
                     // TODO: need to implement way of setting view model for data templates
                     throw new NotImplementedException();
                 }
             });

            // attempt to create from cache first
            object viewModelRef;
            if (this.CreateViewModelFromCache(viewModelName, wpfView, cacheViewModel, out viewModelRef))
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
                    return;
                }
            }

            throw new ViewModelNotResolvedException("View model could not be resolved.", viewModelName);
        }

        private bool ScanDomainForViewModel(string viewName, bool cacheViewModel, out object viewModelRef)
        {
            var filter = new Func<Assembly, bool>(x => !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Microsoft"));

            // search through all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.AsParallel().Where(filter);

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

                // try event first that way consumers could use IoC container and dependance injection if need be
                var item = firstViewModel != null ? this.OnViewModelTypeResolve(new ResolveEventArgs(firstViewModel)) : null;

                // if null attempt direct type creation fallback
                if (item == null)
                {
                    item = firstViewModel != null ? asm.CreateInstance(firstViewModel.FullName) : null;
                }

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

        private bool CreateViewModelFromCache(string name, IView wpfView, bool cacheView, out object viewModelRef)
        {
            if (previouslyCreatedViewModels.ContainsKey(name))
            {
                var viewModelType = previouslyCreatedViewModels[name];

                if (this.GetViewModelFromAssembly(name, viewModelType.Assembly, cacheView, out viewModelRef))
                {
                    return true;
                }
            }

            viewModelRef = null;
            return false;
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
