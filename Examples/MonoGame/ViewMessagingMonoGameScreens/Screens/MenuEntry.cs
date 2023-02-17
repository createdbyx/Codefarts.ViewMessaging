//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// Helper class represents a single entry in a MenuScreen. By default this
/// just draws the entry text string, but it can be customized to display menu
/// entries in different ways. This also provides an event that will be raised
/// when the menu entry is selected.
/// </summary>
class MenuEntry
{
    /// <summary>
    /// Tracks a fading selection effect on the entry.
    /// </summary>
    /// <remarks>
    /// The entries transition out of the selection effect when they are deselected.
    /// </remarks>
    float selectionFade;

    public float SelectionFade
    {
        get
        {
            return this.selectionFade;
        }
    }


    /// <summary>
    /// Gets or sets the text of this menu entry.
    /// </summary>
    public string Text { get; set; }


    /// <summary>
    /// Gets or sets the position at which to draw this menu entry.
    /// </summary>
    public Vector2 Position { get; set; }


    /// <summary>
    /// Event raised when the menu entry is selected.
    /// </summary>
    public event EventHandler<PlayerIndexEventArgs> Selected;


    /// <summary>
    /// Method for raising the Selected event.
    /// </summary>
    protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
    {
        this.Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
    }


    /// <summary>
    /// Constructs a new menu entry with the specified text.
    /// </summary>
    public MenuEntry(string text)
    {
        this.Text = text;
    }


    /// <summary>
    /// Updates the menu entry.
    /// </summary>
    public virtual void Update(MenuScreen screen, bool isSelected, TimeSpan elapsedTime, TimeSpan totalTime)
    {
        // there is no such thing as a selected item on Windows Phone, so we always
        // force isSelected to be false

        // When the menu selection changes, entries gradually fade between
        // their selected and deselected appearance, rather than instantly
        // popping to the new state.
        var fadeSpeed = (float)elapsedTime.TotalSeconds * 4;

        this.selectionFade = isSelected ? Math.Min(this.selectionFade + fadeSpeed, 1) : Math.Max(this.selectionFade - fadeSpeed, 0);
    }
}