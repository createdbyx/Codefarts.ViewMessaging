//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Codefarts.DependencyInjection;
using Codefarts.ScreenManager;
using Microsoft.Xna.Framework;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// The options screen is brought up over the top of the main menu
/// screen, and gives the user a chance to configure the game
/// in various hopefully useful ways.
/// </summary>
class OptionsMenuScreen : MenuScreen
{
    MenuEntry ungulateMenuEntry;
    MenuEntry languageMenuEntry;
    MenuEntry frobnicateMenuEntry;
    MenuEntry elfMenuEntry;

    enum Ungulate
    {
        BactrianCamel,
        Dromedary,
        Llama,
    }

    static Ungulate currentUngulate = Ungulate.Dromedary;

    static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
    static int currentLanguage = 0;

    static bool frobnicate = true;

    static int elf = 23;
    private readonly IDependencyInjectionProvider ioc;


    /// <summary>
    /// Constructor.
    /// </summary>
    public OptionsMenuScreen(IScreenManager manager, IDependencyInjectionProvider ioc) : base(manager, ioc)
    {
        this.MenuTitle = "Options";
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));

        // Create our menu entries.
        this.ungulateMenuEntry = new MenuEntry(string.Empty);
        this.languageMenuEntry = new MenuEntry(string.Empty);
        this.frobnicateMenuEntry = new MenuEntry(string.Empty);
        this.elfMenuEntry = new MenuEntry(string.Empty);

        this.SetMenuEntryText();

        var back = new MenuEntry("Back");

        // Hook up menu event handlers.
        this.ungulateMenuEntry.Selected += this.UngulateMenuEntrySelected;
        this.languageMenuEntry.Selected += this.LanguageMenuEntrySelected;
        this.frobnicateMenuEntry.Selected += this.FrobnicateMenuEntrySelected;
        this.elfMenuEntry.Selected += this.ElfMenuEntrySelected;
        back.Selected += this.OnCancel;

        // Add entries to the menu.
        this.MenuEntries.Add(this.ungulateMenuEntry);
        this.MenuEntries.Add(this.languageMenuEntry);
        this.MenuEntries.Add(this.frobnicateMenuEntry);
        this.MenuEntries.Add(this.elfMenuEntry);
        this.MenuEntries.Add(back);
    }


    /// <summary>
    /// Fills in the latest values for the options screen menu text.
    /// </summary>
    void SetMenuEntryText()
    {
        this.ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
        this.languageMenuEntry.Text = "Language: " + languages[currentLanguage];
        this.frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
        this.elfMenuEntry.Text = "elf: " + elf;
    }


    /// <summary>
    /// Event handler for when the Ungulate menu entry is selected.
    /// </summary>
    void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        currentUngulate++;

        if (currentUngulate > Ungulate.Llama)
        {
            currentUngulate = 0;
        }

        this.SetMenuEntryText();
    }


    /// <summary>
    /// Event handler for when the Language menu entry is selected.
    /// </summary>
    void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        currentLanguage = (currentLanguage + 1) % languages.Length;

        this.SetMenuEntryText();
    }


    /// <summary>
    /// Event handler for when the Frobnicate menu entry is selected.
    /// </summary>
    void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        frobnicate = !frobnicate;

        this.SetMenuEntryText();
    }


    /// <summary>
    /// Event handler for when the Elf menu entry is selected.
    /// </summary>
    void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        elf++;

        this.SetMenuEntryText();
    }
}