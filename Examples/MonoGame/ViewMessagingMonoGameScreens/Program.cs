using Codefarts.AppCore;
using Codefarts.AppCore.Interfaces;
using Codefarts.AppCore.SettingProviders.Xml;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Codefarts.ViewMessaging;
using Codefarts.ViewMessaging.MonoGame.GameScreens;
using ViewMessagingMonoGameScreens;

class Program
{
    public static void Main(string[] args)
    {
        // Register myself as a ioc provider using Container as the actual class that will handle the ioc work
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);

        // register interface endpoints
        var viewService = ioc.Resolve<MonoGameScreenViewService>();
        
        viewService.RegisterView("Background", typeof(BackgroundScreen));
        viewService.RegisterView("Gameplay", typeof(GameplayScreen));
        viewService.RegisterView("Loading", typeof(LoadingScreen));
        viewService.RegisterView("MainMenu", typeof(MainMenuScreen));
        viewService.RegisterView("MessageBox", typeof(MessageBoxScreen));
        viewService.RegisterView("OptionsMenu", typeof(OptionsMenuScreen));
        viewService.RegisterView("PauseMenu", typeof(PauseMenuScreen));
        
        ioc.Register<IViewService>(() => viewService);
        ioc.Register<ISettingsProvider, XmlSettingsProvider>();
        ioc.Register<IPlatformProvider, MonoGamePlatformProvider>();

        // resolve a game object reference
        var game = ioc.Resolve<ViewMessagingGame>();

        // register the game object reference so that code can access it as a singleton reference
        ioc.Register<Microsoft.Xna.Framework.Game>(() => game);

        // run the game
        game.Run();
    }
}