using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using QuartzTypeLib;


namespace FullHDTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        internal const int
            WS_CHILD = 0x40000000,
            WS_VISIBLE = 0x10000000,
            LBS_NOTIFY = 0x00000001,
            HOST_ID = 0x00000002,
            LISTBOX_ID = 0x00000001,
            WS_VSCROLL = 0x00200000,
            WS_BORDER = 0x00800000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private FilgraphManagerClass graphManager = new QuartzTypeLib.FilgraphManagerClass();
        private IMediaControl mControl;
        private IMediaPosition mPosition;
        private IVideoWindow mWindow;
        private KeyboardState oldState;
        private String fileName = @"C:\Users\Ryuzaki\Desktop\smile.mkv"; //ruta del archivo ej: @"C:\Users\Ernesto\Desktop\pelicula.mp4"

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
            // TODO: use this.Content to load your game content here
            
            if(fileName != null){
                graphManager.RenderFile(fileName);
                mControl = graphManager;
                mPosition = graphManager;
                mPosition.Rate = 1.0f;
                mWindow = graphManager;
                mWindow.Owner = Window.Handle.ToInt32();
                mWindow.WindowStyle = WS_CHILD;
                mWindow.SetWindowPosition(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
                graphManager.Run();
                System.Diagnostics.Debug.WriteLine(
                    graphManager.SourceHeight);
                System.Diagnostics.Debug.WriteLine(
                    graphManager.SourceWidth);
            }
            oldState = Keyboard.GetState();
        }

        void videoPlayer_OnVideoComplete(object sender, EventArgs e)
        {
            // Close the app once the video has finished
            Exit();
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
            KeyboardState newState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || newState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                this.Exit();

            

            if (oldState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && newState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Left)) {
                graphManager.Rate -= 0.1;
                System.Diagnostics.Debug.WriteLine("Playback rate: " + graphManager.Rate);
            }

            if (oldState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && newState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                graphManager.Rate += 0.1;
                System.Diagnostics.Debug.WriteLine("Playback rate: " + graphManager.Rate);
            }

            oldState = newState;

            
            
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
