using Codefarts.AppCore.Interfaces;
using Codefarts.AppCore.SettingProviders.Xml;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
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
        ioc.Register<IViewService, MonoGameScreenViewService>();
        ioc.Register<ISettingsProvider, XmlSettingsProvider>();

        // resolve a game object reference
        var game = ioc.Resolve<ViewMessagingGame>();
        
        // register the game object reference so that code can access it as a singleton reference
        ioc.Register<Microsoft.Xna.Framework.Game>(() => game);
        
        // run the game
        game.Run();
    }
}