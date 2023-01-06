//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


namespace BasicGameScreens
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class GameStateManagementGame : Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameStateManagementGame()
        {
            this.Content.RootDirectory = "Content";

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 853;
            this.graphics.PreferredBackBufferHeight = 480;
            this.Window.AllowUserResizing = true;

            // Create the screen manager component.
            this.screenManager = new ScreenManager(this);

            this.Components.Add(this.screenManager);

            // Activate the first screens.
            this.screenManager.AddScreen(new BackgroundScreen(), null);
            this.screenManager.AddScreen(new MainMenuScreen(), null);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (var asset in preloadAssets)
            {
                this.Content.Load<object>(asset);
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }
}