// <copyright file="MainMenuViewModel.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace SimpleConsoleExampleApp.ViewModels
{
    using System;
    using Codefarts.ViewMessaging;

    public class MainMenuViewModel
    {
        public void Quit()
        {
            Environment.Exit(0);
        }

        public void ShowSubMenu()
        {
            var viewService = AppDomain.CurrentDomain.GetData("ViewService") as IViewService;
            var menu = viewService.CreateView("SubMenu");
            viewService.SendMessage(GenericMessageConstants.Show, menu, null);
        }
    }
}
