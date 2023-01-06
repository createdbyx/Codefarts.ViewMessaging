// <copyright file="ShowScreenMessage.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using BasicGameScreens;

namespace Codefarts.ViewMessaging.MonoGame.GameScreens;

using System;

public class ShowScreenMessage : IViewMessage
{
    public string MessageName
    {
        get
        {
            return GenericMessageConstants.Show;
        }
    }

    public void SendMessage(IView view, ViewArguments args)
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        var ctrl = view.ViewReference as GameScreen;
        if (ctrl == null)
        {
            throw new ArgumentException($"ViewReference property is not a {nameof(GameScreen)} type.", nameof(view));
        }

        ctrl.ScreenManager.AddScreen(ctrl, null);
    }
}