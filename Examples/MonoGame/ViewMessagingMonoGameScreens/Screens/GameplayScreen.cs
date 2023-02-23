//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.Input.Models;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// This screen implements the actual game logic. It is just a
/// placeholder to get the idea across: you'll probably want to
/// put some more interesting gameplay in here!
/// </summary>
class GameplayScreen : GameScreen
{
    ContentManager content;
    SpriteFont gameFont;

    Vector2 playerPosition = new Vector2(100, 100);
    Vector2 enemyPosition = new Vector2(100, 100);
    private Vector2 playerDirection = Vector2.Zero;

    Random random = new Random();

    float pauseAlpha;
    private readonly Game game;
    private readonly SpriteBatch spriteBatch;
    Texture2D blankTexture;
    private readonly BindingCallbacksManager input;
    private readonly IDependencyInjectionProvider ioc;
    private float maxMovementSpeed = 150f;


    /// <summary>
    /// Constructor.
    /// </summary>
    public GameplayScreen(IScreenManager manager, IDependencyInjectionProvider ioc) : base(manager)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));

        this.game = ioc.Resolve<Game>();
        this.TransitionOnTime = TimeSpan.FromSeconds(1.5);
        this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        this.spriteBatch = new SpriteBatch(this.game.GraphicsDevice);

        this.input = ioc.Resolve<BindingCallbacksManager>();
        this.input.Bind(Constants.MoveUp, this.MoveUp);
        this.input.Bind(Constants.MoveDown, this.MoveDown);
        this.input.Bind(Constants.MoveLeft, this.MoveLeft);
        this.input.Bind(Constants.MoveRight, this.MoveRight);
        this.input.Bind(Constants.MoveHorizontialy, this.MoveHoriz);
        this.input.Bind(Constants.MoveVerticaly, this.MoveVert);
        this.input.BindButtonRelease(Constants.Pause, this.Pause);
    }

    /// <summary>
    /// Load graphics content for the game.
    /// </summary>
    public override void LoadContent()
    {
        this.content ??= new ContentManager(this.game.Services, "Content");
        this.content.RootDirectory = this.game.Content.RootDirectory;

        this.gameFont = this.content.Load<SpriteFont>("gamefont");
        this.blankTexture = this.content.Load<Texture2D>("blank");

        // A real game would probably have more content than this sample, so
        // it would take longer to load. We simulate that by delaying for a
        // while, giving you a chance to admire the beautiful loading screen.
        Thread.Sleep(1000);

        // once the load has finished, we use ResetElapsedTime to tell the game's
        // timing mechanism that we have just finished a very long frame, and that
        // it should not try to catch up.
        this.game.ResetElapsedTime();
    }


    /// <summary>
    /// Unload graphics content used by the game.
    /// </summary>
    public override void UnloadContent()
    {
        this.content.Unload();
    }


    /// <summary>
    /// Updates the state of the game. This method checks the GameScreen.IsActive
    /// property, so the game will stop updating when the pause menu is active,
    /// or if you tab away to a different application.
    /// </summary>
    public override void Update(TimeSpan elapsedTime, TimeSpan totalTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(elapsedTime, totalTime, otherScreenHasFocus, false);

        // Gradually fade in or out depending on whether we are covered by the pause screen.
        this.pauseAlpha = coveredByOtherScreen ? Math.Min(this.pauseAlpha + 1f / 32, 1) : Math.Max(this.pauseAlpha - 1f / 32, 0);

        if (!this.IsActive)
        {
            return;
        }

        // Apply some random jitter to make the enemy move around.
        const float randomization = 10;

        this.enemyPosition.X += (float)(this.random.NextDouble() - 0.5) * randomization;
        this.enemyPosition.Y += (float)(this.random.NextDouble() - 0.5) * randomization;

        // Apply a stabilizing force to stop the enemy moving off the screen.
        var targetPosition = new Vector2(this.game.GraphicsDevice.Viewport.Width / 2 - this.gameFont.MeasureString("Insert Gameplay Here").X / 2,
                                         200);

        this.enemyPosition = Vector2.Lerp(this.enemyPosition, targetPosition, 0.05f);

        // TODO: this game isn't very fun! You could probably improve
        // it by inserting something more interesting in this space :-)

        var normalizedDir = this.playerDirection;

        normalizedDir.X = (float)(Math.Clamp(normalizedDir.X * this.maxMovementSpeed, -this.maxMovementSpeed, this.maxMovementSpeed) *
                                  elapsedTime.TotalSeconds);
        normalizedDir.Y = (float)(Math.Clamp(normalizedDir.Y * this.maxMovementSpeed, -this.maxMovementSpeed, this.maxMovementSpeed) *
                                  elapsedTime.TotalSeconds);

        this.playerPosition += normalizedDir;  
    }

    public void Pause(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.ScreenManager.AddScreen(this.ioc.Resolve<PauseMenuScreen>(), this.ControllingPlayer);
    }

    public void MoveUp(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.Y += -data.RelativeValue;  
    }

    public void MoveDown(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.Y += data.RelativeValue;  
    }

    public void MoveLeft(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.X += -data.RelativeValue;
    }

    public void MoveRight(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.X += data.RelativeValue;
    }

    private void MoveHoriz(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.X += data.RelativeValue;  
    }


    private void MoveVert(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.playerDirection.Y += -data.RelativeValue;  
    }


    /// <summary>
    /// Draws the gameplay screen.
    /// </summary>
    public override void Draw(TimeSpan elapsedTime, TimeSpan totalTime)
    {
        this.spriteBatch.Begin();

        this.spriteBatch.DrawString(this.gameFont, $"MoveSpeed: {this.playerDirection}\r\n" +
                                                   $"speed: {this.maxMovementSpeed}\r\n" +
                                                   $"{elapsedTime.TotalSeconds}\r\n" +
                                                   $"Dir: {this.playerDirection} - Pos: {this.playerPosition}", Vector2.Zero, Color.Red);

        this.spriteBatch.DrawString(this.gameFont, "// TODO", this.playerPosition, Color.Green);

        this.spriteBatch.DrawString(this.gameFont, "Insert Gameplay Here", this.enemyPosition, Color.DarkRed);


        // If the game is transitioning on or off, fade it out to black.
        if (this.TransitionPosition > 0 || this.pauseAlpha > 0)
        {
            var alpha = MathHelper.Lerp(1f - this.TransitionAlpha, 1f, this.pauseAlpha / 2);

            this.FadeBackBufferToBlack(alpha);
        }

        this.spriteBatch.End();
    }

    /// <summary>
    /// Helper draws a translucent black fullscreen sprite, used for fading
    /// screens in and out, and for darkening the background behind popups.
    /// </summary>
    public void FadeBackBufferToBlack(float alpha)
    {
        var viewport = this.game.GraphicsDevice.Viewport;
        this.spriteBatch.Draw(this.blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
    }
}