// <copyright file="MainMenuView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace SimpleConsoleExampleApp
{
    using System;
    using SimpleConsoleExampleApp.ViewModels;

    public class MainMenuView : BaseConsoleView<MainMenuViewModel>
    {
        public override void DrawMenu()
        {
            Console.WriteLine("==== MAIN MENU ====");
            Console.WriteLine();
            Console.WriteLine("1) Sub Menu");
            Console.WriteLine("2) Quit");
        }

        public override void HandleInput()
        {
            var key = Console.ReadKey();
            var vModel = this.DataContext as MainMenuViewModel;
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    vModel.ShowSubMenu();
                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    vModel.Quit();

                    break;
            }
        }
    }
}