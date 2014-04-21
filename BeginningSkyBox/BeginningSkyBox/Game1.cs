using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PanoEngine;

namespace BeginningSkyBox
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public enum GAME_STATE { GAME };
        public GAME_STATE currentGameState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;
        
        Cubo3D cubo;
        Cuadrado3DClick cuadClick;

        private int numeroEscena = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1024;

            camera = new Camera(this, Vector3.Zero, new Vector3(0,0,-1), Vector3.Up);
            Components.Add(camera);

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
            cuadClick = new Cuadrado3DClick(GraphicsDevice, Content.Load<Texture2D>(@"Texture\TestSprite"));;
            cuadClick.ClickEvent = new Cuadrado3DClick.ClickEventHandler(ClickEventHandler);
            cuadClick.Scale = Matrix.CreateScale(0.4f);
            cuadClick.Position = new Vector3(0, 0, -4.99f);

            
            cubo = new Cubo3D(GraphicsDevice, ObtenerTexturasPorEscena(numeroEscena), false);
            cubo.Scale = Matrix.CreateScale(5);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keyState = Keyboard.GetState();

            float movRad = 0.01f;
            if (keyState.IsKeyDown(Keys.Left)) {
                camera.RotateLookAt(movRad, Camera.AXIS.Y);
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                camera.RotateLookAt(-movRad, Camera.AXIS.Y);
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                camera.RotateLookAt(-movRad, Camera.AXIS.X);
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                camera.RotateLookAt(movRad, Camera.AXIS.X);
            }
            
            
            //camera.Position = distance * new Vector3((float)Math.Sin(angleH), (float)Math.Sin(angleV), (float)Math.Cos(angleH));




            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = GraphicsDevice.Viewport;

            /*if (Intersects(mouseLocation, sprite, camera, viewport) && actualState.LeftButton == ButtonState.Pressed) {
                if (sprite.Visible == true)
                    sprite.Visible = false;
                else {
                    sprite.Visible = true;
                }
            }*/

            cuadClick.Update(gameTime, camera);
            cubo.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            cubo.Draw(camera);
            cuadClick.Draw(camera);


            base.Draw(gameTime);
        }

        #region Helper Methods

        public void ChangeGameState(GAME_STATE state) {
            //Eliminar de la lista de componentes el GameComponent actual

            //Insertar en la lista de componentes el GameComponent asociado al estado de la entrada

            //Modificar el estado actual
            currentGameState = state;
        }

        private void ClickEventHandler(GameTime gameTime) {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.Second);
            cubo.BeginAnimation(ObtenerTexturasPorEscena(++numeroEscena % 2), 3, gameTime, Content.Load<Effect>(@"Effects\TransEffect"));
            
        }

        private Texture2D[] ObtenerTexturasPorEscena(int numeroEscena) {
            Texture2D[] listaTexturas = new Texture2D[6];

            switch (numeroEscena) { 
                case 0:
                    listaTexturas[0] = Content.Load<Texture2D>(@"Texture\Front");
                    listaTexturas[1] = Content.Load<Texture2D>(@"Texture\Right");
                    listaTexturas[2] = Content.Load<Texture2D>(@"Texture\Back");
                    listaTexturas[3] = Content.Load<Texture2D>(@"Texture\Left");
                    listaTexturas[4] = Content.Load<Texture2D>(@"Texture\Top");
                    listaTexturas[5] = Content.Load<Texture2D>(@"Texture\Bot");
                    
                    break;
                case 1:
                    listaTexturas[0] = Content.Load<Texture2D>(@"Texture\img01_front");
                    listaTexturas[1] = Content.Load<Texture2D>(@"Texture\img01_right");
                    listaTexturas[2] = Content.Load<Texture2D>(@"Texture\img01_back");
                    listaTexturas[3] = Content.Load<Texture2D>(@"Texture\img01_left");
                    listaTexturas[4] = Content.Load<Texture2D>(@"Texture\Top");
                    listaTexturas[5] = Content.Load<Texture2D>(@"Texture\Bot");
                    break;
                default:
                    listaTexturas[0] = Content.Load<Texture2D>(@"Texture\Front");
                    listaTexturas[1] = Content.Load<Texture2D>(@"Texture\Right");
                    listaTexturas[2] = Content.Load<Texture2D>(@"Texture\Back");
                    listaTexturas[3] = Content.Load<Texture2D>(@"Texture\Left");
                    listaTexturas[4] = Content.Load<Texture2D>(@"Texture\Top");
                    listaTexturas[5] = Content.Load<Texture2D>(@"Texture\Bot");
                    break;
            }
            return listaTexturas;
        }

        #endregion
    }
}
