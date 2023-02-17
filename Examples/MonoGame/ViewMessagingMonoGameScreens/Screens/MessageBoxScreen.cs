//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameScreen = Codefarts.ScreenManager.MonoGame.GameScreen;

namespace ViewMessagingMonoGameScreens;

/// <summary>
/// A popup message box screen, used to display "are you sure?"
/// confirmation messages.
/// </summary>
class MessageBoxScreen : GameScreen
{
    Texture2D gradientTexture;
    private readonly Game game;
    private ContentManager content;
    private SpriteBatch spriteBatch;
    private readonly IDependencyInjectionProvider ioc;

    public string Message { get; set; }

    private SpriteFont font;
    private Texture2D blankTexture;
    private readonly BindingCallbacksManager input;


    public event EventHandler<PlayerIndexEventArgs> Accepted;
    public event EventHandler<PlayerIndexEventArgs> Cancelled;


    /// <summary>
    /// Constructor automatically includes the standard "A=ok, B=cancel"
    /// usage text prompt.
    /// </summary>
    public MessageBoxScreen(IScreenManager manager,  IDependencyInjectionProvider ioc) : base(manager)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        this.game = ioc.Resolve<Game>();

        this.IsPopup = true;

        this.TransitionOnTime = TimeSpan.FromSeconds(0.2);
        this.TransitionOffTime = TimeSpan.FromSeconds(0.2);
      
        this.input = ioc.Resolve<BindingCallbacksManager>();
        this.input.BindButtonRelease(Constants.MenuExit, e => this.OnCancelled(new PlayerIndexEventArgs((PlayerIndex)e.Player)));
        this.input.BindButtonRelease(Constants.MenuSelect, e => this.OnAccepted(new PlayerIndexEventArgs((PlayerIndex)e.Player)));
    }

    /// <summary>
    /// Loads graphics content for this screen. This uses the shared ContentManager
    /// provided by the Game class, so the content will remain loaded forever.
    /// Whenever a subsequent MessageBoxScreen tries to load this same content,
    /// it will just get back another reference to the already loaded data.
    /// </summary>
    public override void LoadContent()
    {
        this.content ??= new ContentManager(this.game.Services, "Content");
        this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
        this.font = this.content.Load<SpriteFont>("menufont");
        this.blankTexture = this.content.Load<Texture2D>("blank");
        this.gradientTexture = content.Load<Texture2D>("gradient");
    }

    /// <summary>
    /// Unload graphics content used by the game.
    /// </summary>
    public override void UnloadContent()
    {
        this.content.Unload();
    }

    /*
 /// <summary>
 /// Responds to user input, accepting or cancelling the message box.
 /// </summary>
 public override void HandleInput(InputState input)
 {
     PlayerIndex playerIndex;

     // We pass in our ControllingPlayer, which may either be null (to
     // accept input from any player) or a specific index. If we pass a null
     // controlling player, the InputState helper returns to us which player
     // actually provided the input. We pass that through to our Accepted and
     // Cancelled events, so they can tell which player triggered them.
     if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
     {
         // Raise the accepted event, then exit the message box.
         if (this.Accepted != null) this.Accepted(this, new PlayerIndexEventArgs(playerIndex));

         this.ExitScreen();
     }
     else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
     {
         // Raise the cancelled event, then exit the message box.
         if (this.Cancelled != null) this.Cancelled(this, new PlayerIndexEventArgs(playerIndex));

         this.ExitScreen();
     }
 }    */


    /// <summary>
    /// Draws the message box.
    /// </summary>
    public override void Draw(TimeSpan elapsedTime, TimeSpan totalTime)
    {
      
        // Center the message text in the viewport.
        var viewport = this.game.GraphicsDevice.Viewport;
        var viewportSize = new Vector2(viewport.Width, viewport.Height);
        var textSize = font.MeasureString(this.Message);
        var textPosition = (viewportSize - textSize) / 2;

        // The background includes a border somewhat larger than the text itself.
        const int hPad = 32;
        const int vPad = 16;

        var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                (int)textPosition.Y - vPad,
                                                (int)textSize.X + hPad * 2,
                                                (int)textSize.Y + vPad * 2);

        // Fade the popup alpha during transitions.
        var color = Color.White * this.TransitionAlpha;

        spriteBatch.Begin();
        // Darken down any other screens that were drawn beneath the popup.
        this.FadeBackBufferToBlack(this.TransitionAlpha * 2 / 3);

        // Draw the background rectangle.
        spriteBatch.Draw(this.gradientTexture, backgroundRectangle, color);

        // Draw the message box text.
        spriteBatch.DrawString(font, this.Message, textPosition, color);

        spriteBatch.End();
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

    protected virtual void OnAccepted(PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }

        this.Accepted?.Invoke(this, e);
        this.ExitScreen();
    }

    protected virtual void OnCancelled(PlayerIndexEventArgs e)
    {
        if (!this.IsActive)
        {
            return;
        }
       
        this.Cancelled?.Invoke(this, e);
        this.ExitScreen();
    }
}