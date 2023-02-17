//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// Base class for screens that contain a menu of options. The user can
/// move up and down to select an entry, or cancel to back out of the screen.
/// </summary>
abstract class MenuScreen : GameScreen
{
    List<MenuEntry> menuEntries = new List<MenuEntry>();

    int selectedEntry = 0;

    private readonly Game game;
    private SpriteBatch spriteBatch;
    private ContentManager content;
    private SpriteFont font;
    private readonly IDependencyInjectionProvider ioc;
    private readonly BindingCallbacksManager input;

    public Game Game
    {
        get
        {
            return this.game;
        }
    }

    // string menuTitle;
    public string MenuTitle { get; set; }

    /// <summary>
    /// Gets the list of menu entries, so derived classes can add
    /// or change the menu contents.
    /// </summary>
    protected IList<MenuEntry> MenuEntries
    {
        get
        {
            return this.menuEntries;
        }
    }


    /// <summary>
    /// Constructor.
    /// </summary>
    protected MenuScreen(IScreenManager screenManager, IDependencyInjectionProvider ioc) : base(screenManager)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        this.game = ioc.Resolve<Game>();

        // this.menuTitle = menuTitle;
        // this.game = game ?? throw new ArgumentNullException(nameof(game));
        this.content = new ContentManager(game.Services);
        this.content.RootDirectory = game.Content.RootDirectory;
        this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
        this.TransitionOffTime = TimeSpan.FromSeconds(0.5);

        this.input = ioc.Resolve<BindingCallbacksManager>();
        this.input.BindButtonRelease(Constants.MenuUp, e =>
        {
            if (!this.IsActive)
            {
                return;
            }
        
            this.selectedEntry--;

            if (this.selectedEntry < 0) this.selectedEntry = this.menuEntries.Count - 1;
        });
        this.input.BindButtonRelease(Constants.MenuDown, e =>
        {
            if (!this.IsActive)
            {
                return;
            }
        
            this.selectedEntry++;

            if (this.selectedEntry >= this.menuEntries.Count) this.selectedEntry = 0;
        });

        this.input.BindButtonRelease(Constants.MenuExit, e => this.OnCancel((PlayerIndex)e.Player));
        this.input.BindButtonRelease(Constants.MenuSelect, e => this.OnSelectEntry(this.selectedEntry, (PlayerIndex)e.Player));
    }

    public override void LoadContent()
    {
        this.spriteBatch = new SpriteBatch(this.game.GraphicsDevice);
        this.font = this.content.Load<SpriteFont>("menufont");
    }

    public override void UnloadContent()
    {
        this.content.Unload();
    }

    /*
  /// <summary>
  /// Responds to user input, changing the selected entry and accepting
  /// or cancelling the menu.
  /// </summary>
  public   void HandleInput(InputState input)
  {
      // Move to the previous menu entry?
      if (input.IsMenuUp(this.ControllingPlayer))
      {
          this.selectedEntry--;

          if (this.selectedEntry < 0) this.selectedEntry = this.menuEntries.Count - 1;
      }

      // Move to the next menu entry?
      if (input.IsMenuDown(this.ControllingPlayer))
      {
          this.selectedEntry++;

          if (this.selectedEntry >= this.menuEntries.Count) this.selectedEntry = 0;
      }

      // Accept or cancel the menu? We pass in our ControllingPlayer, which may
      // either be null (to accept input from any player) or a specific index.
      // If we pass a null controlling player, the InputState helper returns to
      // us which player actually provided the input. We pass that through to
      // OnSelectEntry and OnCancel, so they can tell which player triggered them.
      PlayerIndex playerIndex;

      if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
      {
          this.OnSelectEntry(this.selectedEntry, playerIndex);
      }
      else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
      {
          this.OnCancel(playerIndex);
      }
  }
           */

    /// <summary>
    /// Handler for when the user has chosen a menu entry.
    /// </summary>
    protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        this.menuEntries[entryIndex].OnSelectEntry(playerIndex);
    }


    /// <summary>
    /// Handler for when the user has cancelled the menu.
    /// </summary>
    protected virtual void OnCancel(PlayerIndex playerIndex)
    {
        if (!this.IsActive)
        {
            return;
        }
        
        this.ExitScreen();
    }


    /// <summary>
    /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
    /// </summary>
    protected void OnCancel(object sender, PlayerIndexEventArgs e)
    {
        this.OnCancel(e.PlayerIndex);
    }


    /// <summary>
    /// Allows the screen the chance to position the menu entries. By default
    /// all menu entries are lined up in a vertical list, centered on the screen.
    /// </summary>
    protected virtual void UpdateMenuEntryLocations()
    {
        // Make the menu slide into place during transitions, using a
        // power curve to make things look more interesting (this makes
        // the movement slow down as it nears the end).
        var transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

        // start at Y = 175; each X value is generated per entry
        var position = new Vector2(0f, 175f);

        // update each menu entry's location in turn
        for (var i = 0; i < this.menuEntries.Count; i++)
        {
            var menuEntry = this.menuEntries[i];

            // each entry is to be centered horizontally
            position.X = this.game.GraphicsDevice.Viewport.Width / 2 - GetWidth(menuEntry) / 2;

            if (this.ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // set the entry's position
            menuEntry.Position = position;

            // move down for the next entry the size of this entry
            position.Y += this.font.LineSpacing; //menuEntry.GetHeight(this);
        }
    }


    /// <summary>
    /// Updates the menu.
    /// </summary>
    public override void Update(TimeSpan elapsedTime, TimeSpan totalTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(elapsedTime, totalTime, otherScreenHasFocus, coveredByOtherScreen);

        // Update each nested MenuEntry object.
        for (var i = 0; i < this.menuEntries.Count; i++)
        {
            var isSelected = this.IsActive && (i == this.selectedEntry);

            this.menuEntries[i].Update(this, isSelected, elapsedTime, totalTime);
        }
    }


    /// <summary>
    /// Draws the menu.
    /// </summary>
    public override void Draw(TimeSpan elapsedTime, TimeSpan totalTime)
    {
        // make sure our entries are in the right place before we draw them
        this.UpdateMenuEntryLocations();

        //var graphics = this.ScreenManager.GraphicsDevice;
        //var spriteBatch = this.ScreenManager.SpriteBatch;
        //var font = this.ScreenManager.Font;

        this.spriteBatch.Begin();

        // Draw each menu entry in turn.
        for (var i = 0; i < this.menuEntries.Count; i++)
        {
            var menuEntry = this.menuEntries[i];

            var isSelected = this.IsActive && (i == this.selectedEntry);

            DrawMenuItem(menuEntry, isSelected, elapsedTime, totalTime);
        }

        // Make the menu slide into place during transitions, using a
        // power curve to make things look more interesting (this makes
        // the movement slow down as it nears the end).
        var transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

        // Draw the menu title centered on the screen
        var viewport = this.game.GraphicsDevice.Viewport;
        var titlePosition = new Vector2(viewport.Width / 2, 80);
        var titleOrigin = font.MeasureString(this.MenuTitle) / 2;
        var titleColor = new Color(192, 192, 192) * this.TransitionAlpha;
        var titleScale = 1.25f;

        titlePosition.Y -= transitionOffset * 100;

        this.spriteBatch.DrawString(font, this.MenuTitle, titlePosition, titleColor, 0,
                                    titleOrigin, titleScale, SpriteEffects.None, 0);

        this.spriteBatch.End();
    }


    /// <summary>
    /// Draws the menu entry. This can be overridden to customize the appearance.
    /// </summary>
    public void DrawMenuItem(MenuEntry entry, bool isSelected, TimeSpan elapsedTime, TimeSpan totalTime)
    {
        // there is no such thing as a selected item on Windows Phone, so we always
        // force isSelected to be false

        // Draw the selected entry in yellow, otherwise white.
        var color = isSelected ? Color.Yellow : Color.White;

        // Pulsate the size of the selected menu entry.
        var time = totalTime.TotalSeconds;

        var pulsate = (float)Math.Sin(time * 6) + 1;

        var scale = 1 + pulsate * 0.05f * entry.SelectionFade;

        // Modify the alpha to fade text out during transitions.
        color *= this.TransitionAlpha;

        // Draw text, centered on the middle of each line.
        // var screenManager = this.ScreenManager;
        // var spriteBatch = screenManager.SpriteBatch;
        // var font = screenManager.Font;

        var origin = new Vector2(0, this.font.LineSpacing / 2);

        this.spriteBatch.DrawString(this.font, entry.Text, entry.Position, color, 0,
                                    origin, scale, SpriteEffects.None, 0);
    }


    // /// <summary>
    // /// Queries how much space this menu entry requires.
    // /// </summary>
    // public   int GetHeight(MenuScreen screen)
    // {
    //     return this.font.LineSpacing;
    // }


    /// <summary>
    /// Queries how wide the entry is, used for centering on the screen.
    /// </summary>
    public int GetWidth(MenuEntry entry)
    {
        return (int)this.font.MeasureString(entry.Text).X;
    }
}