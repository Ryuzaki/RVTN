using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PanoEngine;
using System.Timers;

namespace PaseoSurface
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class App1 : Microsoft.Xna.Framework.Game
    {
        /** GENERAL **/
        private enum STATE { LOADING, PANO};
        private STATE currentState = STATE.LOADING;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private TouchTarget touchTarget;
        private Color backgroundColor = new Color(81, 81, 81);
        private bool applicationLoadCompleteSignalled;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Matrix screenTransform = Matrix.Identity;

        /** LOADING STATE **/
        private Texture2D loadingScreen;
        private Texture2D loadingArrow;
        private float rotationArrow = 0.0f;
        private Timer loadingTimerAnimation;
        /** PANO STATE **/

        /// <summary>
        /// The target receiving all surface input for the application.
        /// </summary>
        protected TouchTarget TouchTarget
        {
            get { return touchTarget; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public App1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        #region Initialization

        /// <summary>
        /// Moves and sizes the window to cover the input surface.
        /// </summary>
        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;

            // Get the window sized right.
            Program.InitializeWindow(Window);
            // Set the graphics device buffers.
            graphics.PreferredBackBufferWidth = Program.WindowSize.Width;
            graphics.PreferredBackBufferHeight = Program.WindowSize.Height;
            graphics.ApplyChanges();
            // Make sure the window is in the right location.
            Program.PositionWindow();
        }

        /// <summary>
        /// Initializes the surface input system. This should be called after any window
        /// initialization is done, and should only be called once.
        /// </summary>
        private void InitializeSurfaceInput()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(touchTarget == null,
                "Surface input already initialized");
            if (touchTarget != null)
                return;

            // Create a target for surface input.
            touchTarget = new TouchTarget(Window.Handle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();

            touchTarget.TouchDown += new EventHandler<TouchEventArgs>(touchTarget_TouchDown);
        }

        void touchTarget_TouchDown(object sender, TouchEventArgs e)
        {
            PanoEngine.PanoEngine.PEPointerPressed = true;
            PanoEngine.PanoEngine.PEPointerX = (int)e.TouchPoint.X;
            PanoEngine.PanoEngine.PEPointerY = (int)e.TouchPoint.Y;
        }

        #endregion

        #region Overridden Game Methods

        /// <summary>
        /// Allows the app to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true; // easier for debugging not to "lose" mouse
            SetWindowOnSurface();
            InitializeSurfaceInput();

            // Set the application's orientation based on the orientation at launch
            currentOrientation = ApplicationServices.InitialOrientation;

            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;

            // Setup the UI to transform if the UI is rotated.
            // Create a rotation matrix to orient the screen so it is viewed correctly
            // when the user orientation is 180 degress different.
            Matrix inverted = Matrix.CreateRotationZ(MathHelper.ToRadians(180)) *
                       Matrix.CreateTranslation(graphics.GraphicsDevice.Viewport.Width,
                                                 graphics.GraphicsDevice.Viewport.Height,
                                                 0);

            if (currentOrientation == UserOrientation.Top)
            {
                screenTransform = inverted;
            }

            //Custom Initialize
            PanoEngine.PanoEngine.PEInitialize(this);
            PanoEngine.PanoEngine.PECreatePaseoVirtual("PaseoVirtual00.xml");

            loadingTimerAnimation = new Timer(40); //25 fps
            loadingTimerAnimation.Elapsed += new ElapsedEventHandler(loadingTimerAnimation_Elapsed);
            loadingTimerAnimation.Enabled = true;
            base.Initialize();
        }

        void loadingTimerAnimation_Elapsed(object sender, ElapsedEventArgs e)
        {
            rotationArrow = (rotationArrow + 0.1f) % ((float)Math.PI * 2);
        }

        /// <summary>
        /// LoadContent will be called once per app and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 45);
            // TODO: use this.Content to load your application content here
            loadingScreen = Content.Load<Texture2D>(@"loadingScreen");
            loadingArrow = Content.Load<Texture2D>(@"loadingArrow");
        }

        /// <summary>
        /// UnloadContent will be called once per app and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the app to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentState) 
            {
                case STATE.LOADING:
                    LoadingUpdate(gameTime);
                    break;
                case STATE.PANO:
                    PanoUpdate(gameTime);
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        protected void LoadingUpdate(GameTime gameTime)
        {
            if (PanoEngine.PanoEngine.PEPaseoCreated)
            {
                loadingTimerAnimation.Enabled = false;
                currentState = STATE.PANO;
            }
        }

        protected void PanoUpdate(GameTime gameTime)
        {
            if (ApplicationServices.WindowAvailability != WindowAvailability.Unavailable)
            {
                if (ApplicationServices.WindowAvailability == WindowAvailability.Interactive)
                {
                    // TODO: Process touches, 
                    // use the following code to get the state of all current touch points.
                    ReadOnlyTouchPointCollection touches = touchTarget.GetState();

                    //get information about input
                    bool goLeft = false, goRight = false;

                    foreach (TouchPoint tp in touches)
                    {
                        if (tp.X > 0 && tp.X <= 100)
                            goLeft = true;

                        if (tp.X < graphics.PreferredBackBufferWidth && tp.X > graphics.PreferredBackBufferWidth - 100)
                            goRight = true;
                    }

                    float movRad = 0.01f;
                    if (goLeft && !goRight)
                    {
                        PanoEngine.PanoEngine.PECamera.RotateLookAt(movRad, Camera.AXIS.Y);
                    }
                    if (goRight && !goLeft)
                    {
                        PanoEngine.PanoEngine.PECamera.RotateLookAt(-movRad, Camera.AXIS.Y);
                    }

                }

                // TODO: Add your update logic here
                if (PanoEngine.PanoEngine.PEPaseoCreated) PanoEngine.PanoEngine.PEPaseo.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the app should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case STATE.LOADING:
                    LoadingDraw(gameTime);
                    break;
                case STATE.PANO:
                    PanoDraw(gameTime);
                    break;
                default:
                    break;
            }
            base.Draw(gameTime);
        }

        protected void LoadingDraw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(loadingScreen, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.Draw(loadingArrow, 
                new Rectangle(
                    (graphics.PreferredBackBufferWidth - (int)(graphics.PreferredBackBufferHeight * 0.1f)) / 2,
                    (graphics.PreferredBackBufferHeight - (int)(graphics.PreferredBackBufferHeight * 0.1f)) / 2 + 90,
                    (int)(graphics.PreferredBackBufferHeight * 0.1f),
                    (int)(graphics.PreferredBackBufferHeight * 0.1f)),
                    null,
                    Color.White,
                    rotationArrow,
                    new Vector2(loadingArrow.Width / 2,loadingArrow.Height / 2),
                    SpriteEffects.None,0
                    );
            
            spriteBatch.End();
        }

        protected void PanoDraw(GameTime gameTime)
        {
            if (!applicationLoadCompleteSignalled)
            {
                // Dismiss the loading screen now that we are starting to draw
                ApplicationServices.SignalApplicationLoadComplete();
                applicationLoadCompleteSignalled = true;
            }

            //TODO: Rotate the UI based on the value of screenTransform here if desired

            GraphicsDevice.Clear(backgroundColor);

            //TODO: Add your drawing code here
            //TODO: Avoid any expensive logic if application is neither active nor previewed
            if (PanoEngine.PanoEngine.PEPaseoCreated) PanoEngine.PanoEngine.PEPaseo.Draw();
        }


        #endregion

        #region Application Event Handlers

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: Enable audio, animations here

            //TODO: Optionally enable raw image here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: Optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: Disable audio, animations here

            //TODO: Disable raw image if it's enabled
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                IDisposable graphicsDispose = graphics as IDisposable;
                if (graphicsDispose != null)
                {
                    graphicsDispose.Dispose();
                }
                if (touchTarget != null)
                {
                    touchTarget.Dispose();
                    touchTarget = null;
                }
            }

            // Release unmanaged Resources.

            // Set large objects to null to facilitate garbage collection.

            base.Dispose(disposing);
        }

        #endregion
    }
}
