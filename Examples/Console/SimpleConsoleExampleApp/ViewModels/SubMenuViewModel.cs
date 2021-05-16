// <copyright file="SubMenuViewModel.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Linq;

namespace SimpleConsoleExampleApp.ViewModels
{
    using System;
    using Codefarts.ViewMessaging;

    public class SubMenuViewModel
    {
        public void Close()
        {
            var viewService = AppDomain.CurrentDomain.GetData("ViewService") as IViewService;
            var view = viewService.Views.FirstOrDefault(x => x.ViewName.Equals("SubMenu", StringComparison.InvariantCultureIgnoreCase));
            viewService.DeleteView(view.Id);
        }

        public void ShowSubMenu()
        {
            var viewService = AppDomain.CurrentDomain.GetData("ViewService") as IViewService;
            var menu = viewService.CreateView("SubMenu");
            viewService.SendMessage(GenericMessageConstants.Show, menu, null);
        }
    }
}