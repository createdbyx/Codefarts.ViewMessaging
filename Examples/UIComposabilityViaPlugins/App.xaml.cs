using System.Windows;
using Codefarts.ViewMessaging;

namespace UIComposabilityViaPlugins
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private WpfViewService viewService;

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            // create view service
            this.viewService = new WpfViewService();
            this.viewService.MvvmEnabled = true;

            // App.xaml has it's StartupUri removed so we set main window here
            var view = this.viewService.CreateView("MainWindow");
            this.MainWindow = view.ViewReference as Window;

            // create and pass arguments to the view to show the window
            var args = GenericMessageArguments.Show();
            this.viewService.SendMessage(GenericMessageConstants.Show, view, args);

            base.OnStartup(e);
        } }
}