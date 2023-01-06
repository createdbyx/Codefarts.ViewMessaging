// <copyright file="P:\Code Projects\Codefarts.ViewMessaging\Examples\ShowMainAppWindow\App.xaml.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace ShowMainAppWindow
{
    using System.Windows;
    using Codefarts.ViewMessaging;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IViewService viewService;

        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            // create view service
            this.viewService = new WpfViewService();

            // App.xaml has it's StartupUri removed so we set main window here
            var view = this.viewService.CreateView("MainWindow");
            this.MainWindow = view.ViewReference as Window;

            // create and pass arguments to the view to show the window
            var args = GenericMessageArguments.Show();
            this.viewService.SendMessage(GenericMessageConstants.Show, view, args);

            base.OnStartup(e);
        }
    }
}