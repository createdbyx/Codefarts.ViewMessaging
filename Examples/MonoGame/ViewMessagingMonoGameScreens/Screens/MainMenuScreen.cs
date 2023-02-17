//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.ScreenManager;
using Microsoft.Xna.Framework;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// The main menu screen is the first thing displayed when the game starts up.
/// </summary>
class MainMenuScreen : MenuScreen
{
    private readonly IDependencyInjectionProvider ioc;

    /// <summary>
    /// Constructor fills in the menu contents.
    /// </summary>
    public MainMenuScreen(IScreenManager screenManager, IDependencyInjectionProvider ioc) : base(screenManager, ioc)
    {
        this.MenuTitle = "Main Menu";
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));

        // Create our menu entries.
        var playGameMenuEntry = new MenuEntry("Play Game");
        var optionsMenuEntry = new MenuEntry("Options");
        var exitMenuEntry = new MenuEntry("Exit");

        // Hook up menu event handlers.
        playGameMenuEntry.Selected += this.PlayGameMenuEntrySelected;
        optionsMenuEntry.Selected += this.OptionsMenuEntrySelected;
        exitMenuEntry.Selected += this.OnCancel;

        // Add entries to the menu.
        this.MenuEntries.Add(playGameMenuEntry);
        this.MenuEntries.Add(optionsMenuEntry);
        this.MenuEntries.Add(exitMenuEntry);
    }


    /// <summary>
    /// Event handler for when the Play Game menu entry is selected.
    /// </summary>
    void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }

        // Tell all the current screens to transition off.
        foreach (var screen in this.ScreenManager.GetScreens())
        {
            screen.ExitScreen();
        }

        var load = this.ioc.Resolve<LoadingScreen>();
        load.LoadingIsSlow = true;

        var screens = new IGameScreen[1];
        // screens[0] = this.ioc.Resolve<BackgroundScreen>();
        screens[0] = this.ioc.Resolve<GameplayScreen>();
        load.ScreensToLoad = screens;

       this.ScreenManager.AddScreen(load, (int?)e.PlayerIndex);
        //LoadingScreen.Load(this.ScreenManager, true, e.PlayerIndex, new GameplayScreen());
    }


    /// <summary>
    /// Event handler for when the Options menu entry is selected.
    /// </summary>
    void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.ScreenManager.AddScreen(this.ioc.Resolve<OptionsMenuScreen>(), (int?)e.PlayerIndex);
    }


    /// <summary>
    /// When the user cancels the main menu, ask if they want to exit the sample.
    /// </summary>
    protected override void OnCancel(PlayerIndex playerIndex)
    {
        if (!this.IsActive)
        {
            return;
        }

        string message = "Are you sure you want to exit this sample?";

        var iMan = this.ioc.Resolve<InputManager>();
        var menuSelects = iMan.Bindings.Where(x => x.Name.Equals(nameof(Constants.MenuSelect)))
                              .Select(x => new { Input = x.InputSource.Name, Source = x.Source });
        var menuExists = iMan.Bindings.Where(x => x.Name.Equals(nameof(Constants.MenuExit)))
                             .Select(x => new { Input = x.InputSource.Name, Source = x.Source });

        message += "\r\nAccept: " + String.Join(", ", menuSelects.Select(x => $"{x.Source}"));
        message += "\r\nCancel: " + String.Join(", ", menuExists.Select(x => $"{x.Source}"));

        var confirmExitMessageBox = this.ioc.Resolve<MessageBoxScreen>();
        confirmExitMessageBox.Message = message;

        confirmExitMessageBox.Accepted += this.ConfirmExitMessageBoxAccepted;

        this.ScreenManager.AddScreen(confirmExitMessageBox, (int?)playerIndex);
    }


    /// <summary>
    /// Event handler for when the user selects ok on the "are you sure
    /// you want to exit" message box.
    /// </summary>
    void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
    {
        this.Game.Exit();
    }
}