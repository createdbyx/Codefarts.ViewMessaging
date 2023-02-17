//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Threading;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.Input.Models;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    private float moveSpeed = 150f;


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

        if (this.playerDirection.Length() > 0)
        {
            this.playerDirection.Normalize();
        }
        
        this.playerPosition += this.playerDirection * this.moveSpeed * (float)elapsedTime.TotalSeconds;
    }


    /*
    /// <summary>
    /// Lets the game respond to player input. Unlike the Update method,
    /// this will only be called when the gameplay screen is active.
    /// </summary>
    public void HandleInput()
    {
        // Look up inputs for the active player profile.
        var playerIndex = this.ControllingPlayer.Value;

        var keyboardState = this.input.CurrentKeyboardStates[playerIndex];
        var gamePadState = this.input.CurrentGamePadStates[playerIndex];

        // The game pauses either if the user presses the pause button, or if
        // they unplug the active gamepad. This requires us to keep track of
        // whether a gamepad was ever plugged in, because we don't want to pause
        // on PC if they are playing with a keyboard and have no gamepad at all!
        var gamePadDisconnected = !gamePadState.IsConnected && this.input.GamePadWasConnected[playerIndex];

        if (this.input.IsPauseGame((PlayerIndex?)this.ControllingPlayer) || gamePadDisconnected)
        {
            this.ScreenManager.AddScreen(new PauseMenuScreen(), this.ControllingPlayer);
        }
        else
        {
            // Otherwise move the player position.
            var movement = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;

            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;

            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;

            var thumbstick = gamePadState.ThumbSticks.Left;

            movement.X += thumbstick.X;
            movement.Y -= thumbstick.Y;

            if (movement.Length() > 1)
                movement.Normalize();

            this.playerPosition += movement * 2;
        }
    } */

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

        // var speed = -data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerPosition.Y += speed;
        this.playerDirection.Y = -data.RelativeValue;
    }

    public void MoveDown(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        // var speed = data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerPosition.Y += speed;
        this.playerDirection.Y = data.RelativeValue;
    }

    public void MoveLeft(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }
                    
        // var speed = -data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerDirection.X = -data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerPosition += new Vector2(speed, 0);
        this.playerDirection.X = data.RelativeValue!=0? -data.RelativeValue:this.playerDirection.X;
    }

    public void MoveRight(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        //  var speed = data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        //  this.playerDirection.X=  data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        //  this.playerPosition += new Vector2(speed, 0);
      //  this.playerDirection.X = data.RelativeValue;
        this.playerDirection.X = data.RelativeValue!=0? data.RelativeValue:this.playerDirection.X;
    }

    private void MoveHoriz(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        //var speed = data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerDirection.X = (float)(data.Value * this.moveSpeed * data.ElapsedTime.TotalSeconds);
        this.playerDirection.X += data.Value;
    }


    private void MoveVert(BindingData data)
    {
        if (!this.IsActive)
        {
            return;
        }

        //var speed = data.Value * this.moveSpeed * data.ElapsedTime.Milliseconds;
        // this.playerDirection.Y = (float)(-data.Value * this.moveSpeed * data.ElapsedTime.TotalSeconds);
        this.playerDirection.Y += -data.Value;
    }


    /// <summary>
    /// Draws the gameplay screen.
    /// </summary>
    public override void Draw(TimeSpan elapsedTime, TimeSpan totalTime)
    {
        // This game has a blue background. Why? Because!
        // this.game.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

        // Our player and enemy are both actually just text strings.

        this.spriteBatch.Begin();

        this.spriteBatch.DrawString(this.gameFont, $"MoveSpeed: {this.playerDirection}\r\n" +
                                                   $"speed: {this.moveSpeed}\r\n" +
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

        //  this.spriteBatch.Begin();

        this.spriteBatch.Draw(this.blankTexture,
                              new Rectangle(0, 0, viewport.Width, viewport.Height),
                              Color.Black * alpha);

        //  this.spriteBatch.End();
    }
}