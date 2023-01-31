// <copyright file="ViewMessagingGame.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using Codefarts.DependencyInjection;
using Codefarts.ViewMessaging;

namespace ViewMessagingMonoGameScreens;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public class ViewMessagingGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch _spriteBatch;
    private IViewService viewService;
    private IDependencyInjectionProvider provider;

    public ViewMessagingGame(IDependencyInjectionProvider provider)
    {
        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        this.provider = provider;
        this.graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        this.viewService = this.provider.Resolve<IViewService>();
        //var view = this.viewService.CreateView("Game");

        //service.AppendedViewName = "Screen";

        // create and pass arguments to the view to show the window
        //var args = GenericMessageArguments.Show();
        //this.viewService.SendMessage(GenericMessageConstants.Show, view, args);

        this.graphics.PreferredBackBufferWidth = 853;
        this.graphics.PreferredBackBufferHeight = 480;
        this.Window.AllowUserResizing = true;

        // Create the screen manager component.
        // this.screenManager = new ScreenManager(this);

        //this.Components.Add(service);

        // Activate the first screens.
        // this.screenManager.AddScreen(new BackgroundScreen(), null);
        //  this.screenManager.AddScreen(new MainMenuScreen(), null);

        // create and pass arguments to the view in the game window
        var args = GenericMessageArguments.Show();
        var view = this.viewService.CreateView("Background");
        this.viewService.SendMessage(GenericMessageConstants.Show, view, args);

        view = this.viewService.CreateView("MainMenu");
        this.viewService.SendMessage(GenericMessageConstants.Show, view, args);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    } 
}