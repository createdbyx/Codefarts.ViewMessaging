using System.ComponentModel;
using System.Reflection;
using BasicGameScreens;
using Codefarts.DependencyInjection;

namespace Codefarts.ViewMessaging.MonoGame.GameScreens;

public class MonoGameScreenViewService : IViewService, INotifyPropertyChanged
{
    /// <summary>
    /// The cache of previously created view types.
    /// </summary>
    /// <remarks>Only the type for the view is cached to prevent having to search thought the loaded assembly list to find it again.</remarks>
    private static readonly Dictionary<string, Type> previouslyCreatedViews = new Dictionary<string, Type>();

    private IDictionary<string, IViewMessage> messageHandlers = new Dictionary<string, IViewMessage>();
    private readonly IDictionary<string, IView> viewReferences = new Dictionary<string, IView>();
    private string appendedViewName = "View";
    private List<Func<string, ViewArguments, IView>> handlerCallbacks = new List<Func<string, ViewArguments, IView>>();
    ScreenManager screenManager;
    private readonly IDependencyInjectionProvider diProvider;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Occurs every ime a view is created.
    /// </summary>
    public event EventHandler<ViewEventArgs> ViewCreated;

    /// <summary>
    /// Occurs every ime a view is deleted.
    /// </summary>
    public event EventHandler<ViewEventArgs> BeforeViewDeleted;

    /// <summary>
    /// Occurs every ime a view is deleted.
    /// </summary>
    public event EventHandler<ViewDeletedEventArgs> ViewDeleted;

    public MonoGameScreenViewService(IDependencyInjectionProvider provider)
    {
        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        this.diProvider = provider;
        this.screenManager = this.diProvider.Resolve<ScreenManager>();
        this.screenManager.Game.Components.Add(this.screenManager);

        //  this.messageHandlers[GenericMessageConstants.ShowDialog] = new ShowDialogMessage();
        this.messageHandlers[GenericMessageConstants.Show] = new ShowScreenMessage();
        //  this.messageHandlers[GenericMessageConstants.SetModel] = new SetModelMessage();
        //  this.messageHandlers[GenericMessageConstants.Update] = new UpdateMessage();
        // this.messageHandlers[GenericMessageConstants.Refresh] = new RefreshMessage();
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

    public IEnumerable<IView> Views
    {
        get
        {
            return this.viewReferences.Values;
        }
    }

    public IDictionary<string, IViewMessage> MessageHandlers
    {
        get
        {
            return this.messageHandlers;
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
        IView viewRef;
        bool result;
        lock (this.viewReferences)
        {
            this.viewReferences.TryGetValue(id, out viewRef);
            result = this.viewReferences.Remove(id);
        }

        if (result)
        {
            this.OnBeforeViewDeleted(viewRef);
            this.OnViewDeleted(id);
        }

        return result;
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

        //   var isDataTemplate = args.Get<bool>("IsDataTemplate");
        var useCache = args.Get("UseCache", true);
        var scanForAssemblies = args.Get("ScanAssemblies", true);
        var viewName = name + this.appendedViewName;
        //var viewModelName = name + this.appendedViewModelName;

        IView gameScreenView;

        // try to use handlers first
        foreach (var callback in this.handlerCallbacks)
        {
            gameScreenView = callback(name, args);
            if (gameScreenView != null)
            {
                this.OnViewCreated(gameScreenView);
                return gameScreenView;
            }
        }

        // attempt to create from cache first
        if (this.CreateViewFromCache(viewName, args, name, out gameScreenView))
        {
            //this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, wpfView);
            return gameScreenView;
        }

        // if not in cache scan for
        if (this.ScanDomainForView(viewName, args, name, useCache, out gameScreenView))
        {
            // this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, wpfView);
            return gameScreenView;
        }

        if (scanForAssemblies && this.SearchForViewAssemblies(viewName, args, name, useCache, out gameScreenView))
        {
            // this.SetViewModelAndReturn(args, viewModelName, scanForAssemblies, useCache, wpfView);
            return gameScreenView;
        }

        return null;
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


            if (this.GetViewType(viewName, args, assembly, name, cacheView, out wpfView))
            {
                return true;
            }
        }

        wpfView = null;
        return false;
    }

    private bool ScanDomainForView(string viewName, ViewArguments args, string name, bool cacheView, out IView wpfView)
    {
        wpfView = null;

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


        return false;
    }

    private bool GetViewType(string viewName, ViewArguments args, Assembly asm, string name, bool cacheView,
                             out IView wpfView)
    {
        if (asm == null)
        {
            throw new ArgumentNullException(nameof(asm));
        }

        var types = asm.GetTypes().AsParallel();
        var views = types.Where(x => this.ViewTypeAndNameMatch(x, viewName));

        try
        {
            var firstView = views.FirstOrDefault();
//            var item = firstView != null ? asm.CreateInstance(firstView.FullName) : null;
            var item = firstView != null ? this.diProvider.Resolve(firstView) : null;

            if (item != null)
            {
                var element = item as GameScreen;
                var newView = new MonoGameScreenView(this, element, name, args == null ? null : new ViewArguments(args));
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
            return x.IsSubclassOf(typeof(GameScreen));
        }

        return false;
    }

    private bool CreateViewFromCache(string viewName, ViewArguments args, string dataTemplateName, out IView wpfView)
    {
        wpfView = null;

        if (previouslyCreatedViews.ContainsKey(viewName))
        {
            var firstView = previouslyCreatedViews[viewName];

            //var item = firstView != null ? firstView.Assembly.CreateInstance(firstView.FullName) : null;
            var item = firstView != null ? this.diProvider.Resolve(firstView) : null;
            if (item != null)
            {
                var element = item as GameScreen;
                var newView = new MonoGameScreenView(this, element, dataTemplateName, args == null ? null : new ViewArguments(args));
                this.viewReferences.Add(newView.Id, newView);
                {
                    wpfView = newView;
                    return true;
                }
            }
        }

        return false;
    }

    public void RegisterHandler(Func<string, ViewArguments, IView> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        this.handlerCallbacks.Add(callback);
    }

    public void SendMessage(string message, IView view, ViewArguments args)
    {
        this.MessageHandlers[message].SendMessage(view, args);
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
    /// Called before a view is deleted.
    /// </summary>
    /// <param name="view">Reference to the view that is to be.</param>
    protected virtual void OnBeforeViewDeleted(IView view)
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        var handler = this.BeforeViewDeleted;
        if (handler != null)
        {
            handler(this, new ViewEventArgs(view));
        }
    }

    /// <summary>
    /// Called after a view if successfully deleted.
    /// </summary>
    /// <param name="viewId">Reference to the view that was deleted.</param>
    protected virtual void OnViewDeleted(string viewId)
    {
        var handler = this.ViewDeleted;
        if (handler != null)
        {
            handler(this, new ViewDeletedEventArgs(viewId));
        }
    }
}