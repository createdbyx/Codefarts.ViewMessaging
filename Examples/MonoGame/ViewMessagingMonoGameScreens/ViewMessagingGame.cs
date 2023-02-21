// <copyright file="ViewMessagingGame.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Windows.Forms;
using Codefarts.DependencyInjection;
using Codefarts.Input;
using Codefarts.Input.MonoGameSources;
using Codefarts.ScreenManager;
using Codefarts.ScreenManager.MonoGame;
using Codefarts.ViewMessaging;

namespace ViewMessagingMonoGameScreens;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public class ViewMessagingGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager graphics;

    //  private SpriteBatch _spriteBatch;
    private IViewService viewService;
    private IDependencyInjectionProvider provider;
    private IScreenManager screenManager;
    private InputManager inputManager;

    public ViewMessagingGame(IDependencyInjectionProvider provider)
    {
        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        this.provider = provider;
        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;

        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.PreferredBackBufferWidth = 853;
        this.graphics.PreferredBackBufferHeight = 480;
        this.Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        this.inputManager = this.provider.Resolve<InputManager>();
        this.provider.Register<InputManager>(() => this.inputManager);
        this.Services.AddService(this.inputManager);

        this.SetupInputs();

        this.provider.Register<GraphicsDevice>(() => this.GraphicsDevice);
        this.screenManager = this.provider.Resolve<ScreenManager>();
        this.provider.Register(() => this.screenManager);
        this.Components.Add(this.screenManager as IGameComponent);

        this.viewService = this.provider.Resolve<IViewService>();
        //var view = this.viewService.CreateView("Game");

        //service.AppendedViewName = "Screen";

        // create and pass arguments to the view to show the window
        //var args = GenericMessageArguments.Show();
        //this.viewService.SendMessage(GenericMessageConstants.Show, view, args);


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

    private void SetupInputs()
    {
        var kbSource = new KeyboardSource();
        var gpSource = new GamePadSource();

        this.inputManager.AddSource(kbSource);
        this.inputManager.AddSource(gpSource);
        this.inputManager.Bind(Constants.MoveUp, kbSource, "W");
        this.inputManager.Bind(Constants.MoveDown, kbSource, "S");
        this.inputManager.Bind(Constants.MoveLeft, kbSource, "A");
        this.inputManager.Bind(Constants.MoveRight, kbSource, "D");
        this.inputManager.Bind(Constants.Pause, kbSource, "Escape");

        this.inputManager.Bind(Constants.MoveUp, gpSource, "Up");
        this.inputManager.Bind(Constants.MoveDown, gpSource, "Down");
        this.inputManager.Bind(Constants.MoveLeft, gpSource, "Left");
        this.inputManager.Bind(Constants.MoveRight, gpSource, "Right");
        this.inputManager.Bind(Constants.MoveHorizontialy, gpSource, "LeftThumbStickX");
        this.inputManager.Bind(Constants.MoveVerticaly, gpSource, "LeftThumbStickY");
        this.inputManager.Bind(Constants.Pause, gpSource, "Start");

        this.inputManager.Bind(Constants.MenuUp, kbSource, "Up");
        this.inputManager.Bind(Constants.MenuDown, kbSource, "Down");
        this.inputManager.Bind(Constants.MenuExit, kbSource, "Escape");
        this.inputManager.Bind(Constants.MenuSelect, kbSource, "Enter");

        this.inputManager.Bind(Constants.MenuUp, gpSource, "Up");
        this.inputManager.Bind(Constants.MenuDown, gpSource, "Down");
        this.inputManager.Bind(Constants.MenuExit, gpSource, "B");
        this.inputManager.Bind(Constants.MenuSelect, gpSource, "A");
    }

    // protected override void LoadContent()
    // {
    //  //   _spriteBatch = new SpriteBatch(GraphicsDevice);
    //
    //     // TODO: use this.Content to load your game content here
    // }
    //
    protected override void Update(GameTime gameTime)
    {
        // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //     Exit();

        // TODO: Add your update logic here


        base.Update(gameTime);
        this.inputManager.Update(gameTime.ElapsedGameTime, gameTime.TotalGameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}