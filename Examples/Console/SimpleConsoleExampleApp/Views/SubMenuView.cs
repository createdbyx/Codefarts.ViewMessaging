// <copyright file="SubMenuView.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace SimpleConsoleExampleApp
{
    using System;
    using SimpleConsoleExampleApp.ViewModels;

    public class SubMenuView : BaseConsoleView<SubMenuViewModel>
    {
        public override void DrawMenu()
        {
            Console.WriteLine("==== SUB MENU ====");
            Console.WriteLine();
            Console.WriteLine("1) Menu Item 1");
            Console.WriteLine("2) Menu Item 2");
            Console.WriteLine("3) Menu Item 3");
            Console.WriteLine("4) Return to Main Menu");
        }

        public override void HandleInput()
        {
            var key = Console.ReadKey();
            var vModel = this.DataContext as SubMenuViewModel;
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.NumPad1:
                case ConsoleKey.NumPad2:
                case ConsoleKey.NumPad3:
                    // does nothing
                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.D4:
                    vModel.Close();
                    break;
            }
        }
    }
}