// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace SimpleConsoleExampleApp
{
    using System;
    using Codefarts.ViewMessaging;

    class Program
    {
        static void Main(string[] args)
        {
            var viewService = new ConsoleViewService() { MvvmEnabled = true };
            var app = new ConsoleApp(viewService);
            app.Run();
        }

        public class ConsoleApp
        {
            private IViewService viewService;
            private bool isRunning;

            public ConsoleApp(IViewService viewService)
            {
                this.viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            }

            public void Run()
            {
                var menu = this.viewService.CreateView("MainMenu");
                Console.CursorVisible = false;
                this.viewService.SendMessage(GenericMessageConstants.Show, menu, null);

                this.isRunning = true;
                while (this.isRunning)
                {

                }
            }
        }
    }
}
