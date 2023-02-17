//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.ScreenManager;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// The pause menu comes up over the top of the game,
/// giving the player options to resume or quit.
/// </summary>
class PauseMenuScreen : MenuScreen
{
    private readonly IDependencyInjectionProvider ioc;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PauseMenuScreen(IScreenManager manager, IDependencyInjectionProvider ioc) : base(manager, ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));

        this.MenuTitle = "Paused";

        // Create our menu entries.
        var resumeGameMenuEntry = new MenuEntry("Resume Game");
        var quitGameMenuEntry = new MenuEntry("Quit Game");

        // Hook up menu event handlers.
        resumeGameMenuEntry.Selected += this.OnCancel;
        quitGameMenuEntry.Selected += this.QuitGameMenuEntrySelected;

        // Add entries to the menu.
        this.MenuEntries.Add(resumeGameMenuEntry);
        this.MenuEntries.Add(quitGameMenuEntry);
    }

    /// <summary>
    /// Event handler for when the Quit Game menu entry is selected.
    /// </summary>
    void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }

        string message = "Are you sure you want to quit this game?";
        var iMan = this.ioc.Resolve<InputManager>();
        var menuSelects = iMan.Bindings.Where(x => x.Name.Equals(nameof(Constants.MenuSelect)))
                              .Select(x => new { Input = x.InputSource.Name, Source = x.Source });
        var menuExists = iMan.Bindings.Where(x => x.Name.Equals(nameof(Constants.MenuExit)))
                             .Select(x => new { Input = x.InputSource.Name, Source = x.Source });

        message += "\r\nAccept: " + String.Join(", ", menuSelects.Select(x => $"{x.Source}"));
        message += "\r\nCancel: " + String.Join(", ", menuExists.Select(x => $"{x.Source}"));

        var confirmQuitMessageBox = this.ioc.Resolve<MessageBoxScreen>();
        confirmQuitMessageBox.Message = message;

        confirmQuitMessageBox.Accepted += this.ConfirmQuitMessageBoxAccepted;

        this.ScreenManager.AddScreen(confirmQuitMessageBox, this.ControllingPlayer);
    }

    /// <summary>
    /// Event handler for when the user selects ok on the "are you sure
    /// you want to quit" message box. This uses the loading screen to
    /// transition from the game back to the main menu screen.
    /// </summary>
    void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
    {
        // Tell all the current screens to transition off.
        foreach (var screen in this.ScreenManager.GetScreens())
        {
            screen.ExitScreen();
        }

        var load = this.ioc.Resolve<LoadingScreen>();
        load.LoadingIsSlow = false;

        var screens = new IGameScreen[2];
        screens[0] = this.ioc.Resolve<BackgroundScreen>();
        screens[1] = this.ioc.Resolve<MainMenuScreen>();
        load.ScreensToLoad = screens;

        this.ScreenManager.AddScreen(load, null);
    }
}