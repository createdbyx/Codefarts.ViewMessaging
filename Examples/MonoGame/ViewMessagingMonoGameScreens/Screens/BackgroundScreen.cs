//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// The background screen sits behind all the other menu screens.
/// It draws a background image that remains fixed in place regardless
/// of whatever transitions the screens on top of it may be doing.
/// </summary>
class BackgroundScreen : GameScreen
{
    ContentManager content;
    Texture2D backgroundTexture;
    private SpriteBatch spriteBatch;
    private readonly Game game;


    /// <summary>
    /// Constructor.
    /// </summary>
    public BackgroundScreen(IScreenManager manager, Game game) : base(manager)
    {
        this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
        this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        this.game = game ?? throw new ArgumentNullException(nameof(game));
    }


    /// <summary>
    /// Loads graphics content for this screen. The background texture is quite
    /// big, so we use our own local ContentManager to load it. This allows us
    /// to unload before going from the menus into the game itself, wheras if we
    /// used the shared ContentManager provided by the Game class, the content
    /// would remain loaded forever.
    /// </summary>
    public override void LoadContent()
    {
        this.content ??= new ContentManager(this.game.Services, "Content");
        this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
        this.backgroundTexture = this.content.Load<Texture2D>("background");
    }


    /// <summary>
    /// Unloads graphics content for this screen.
    /// </summary>
    public override void UnloadContent()
    {
        this.content.Unload();
    }

    public override void Update(TimeSpan elapsedTime, TimeSpan totalTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(elapsedTime, totalTime, otherScreenHasFocus, false);
    }

    public override void Draw(TimeSpan elapsedTime, TimeSpan totalTime)
    {
        var viewport = this.game.GraphicsDevice.Viewport;
        var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        this.spriteBatch.Begin();

        this.spriteBatch.Draw(this.backgroundTexture, fullscreen,
                              new Color(this.TransitionAlpha, this.TransitionAlpha, this.TransitionAlpha));

        this.spriteBatch.End();
    }
}