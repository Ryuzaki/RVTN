using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PanoEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouseOldState;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            PanoEngine.PEInitialize(this);
            PanoEngine.PECreatePaseoVirtual("PaseoVirtual00.xml");
            mouseOldState = Mouse.GetState();
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 45);
            // TODO: use this.Content to load your game content here
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if(PanoEngine.PEPaseoCreated) PanoEngine.PEPaseo.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keyState = Keyboard.GetState();

            float movRad = 0.01f;
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                PanoEngine.PECamera.RotateLookAt(movRad, Camera.AXIS.Y);
            }
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                PanoEngine.PECamera.RotateLookAt(-movRad, Camera.AXIS.Y);
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                PanoEngine.PECamera.RotateLookAt(-movRad, Camera.AXIS.X);
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                PanoEngine.PECamera.RotateLookAt(movRad, Camera.AXIS.X);
            }

            MouseState newMouseState = Mouse.GetState();
            PanoEngine.PEPointerX = newMouseState.X;
            PanoEngine.PEPointerY = newMouseState.Y;
            PanoEngine.PEPointerPressed = (newMouseState.LeftButton == ButtonState.Pressed);
            if (mouseOldState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released)
            {
                PanoEngine.PEPointerReleased = true;
            }
            else
            {
                PanoEngine.PEPointerReleased = false;
            }
            mouseOldState = newMouseState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (PanoEngine.PEPaseoCreated) PanoEngine.PEPaseo.Draw();
            // TODO: Add your drawing code here
            //space.Draw(PanoEngine.PECamera);
            base.Draw(gameTime);
        }
    }
}
