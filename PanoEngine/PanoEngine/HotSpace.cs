using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PanoEngine
{
    public class HotSpace
    {
        #region Fields

        private GraphicsDevice graphicsDevice;
        private Sprite3D[] faces;
        private BasicEffect effect;
        
        private Matrix world = Matrix.Identity;
        private Matrix rotation = Matrix.Identity;
        private Matrix scale = Matrix.CreateScale(1);
        private Matrix position = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private bool visible = true;

        //Transition and blending animation
        private int? initBlending = null;
        private int blendingDurationMiliseg = 0;
        private float actualRotationRad;
        private float totalRotationRad;

        private String name;
        private List<HotSpot> listaHotSpot;
        public enum WALL { NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3};
        private List<GoTo> listaGoTo;

        #endregion

        #region Properties

        public Matrix World
        {
            get
            {
                return world;
            }
            set
            {
                world = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position.Translation;
            }
            set
            {
                position = Matrix.CreateTranslation(value);
                foreach (Sprite3D cuadrado in faces) {
                    cuadrado.Position += value;
                }
            }
        }

        public Matrix Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                foreach (Sprite3D cuadrado in faces)
                {
                    cuadrado.Scale = value;
                    cuadrado.Position = cuadrado.Position * new Vector3(value.M11, value.M22, value.M33);
                }
            }
        }

        public Matrix Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #endregion

        #region Constructor, Draw and Update

        /// <summary>
        /// Initializes a new instance of the <see cref="HotSpace"/> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="listTextures">Lista de Texture2D correspondiente al frontal, derecha, atras, izquierda, arriba y abajo
        /// respectivamente. </param>
        public HotSpace(String name, String[] listTextures)
        {
            ConstructorHotSpace(name, listTextures);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotSpace"/> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="listTextures">Lista de Texture2D correspondiente al frontal, derecha, atras, izquierda, arriba y abajo
        /// respectivamente. </param>
        public HotSpace(String name)
        {
            ConstructorHotSpace(name, null);
        }

        private void ConstructorHotSpace(String name, String[] listTextures)
        {
            this.graphicsDevice = PanoEngine.PEGraphicsDevice;
            this.name = name;

            //inicializar lista de hotspots
            listaHotSpot = new List<HotSpot>();

            //inicializar lista de gotos
            listaGoTo = new List<GoTo>();

            //Inicializar caras del hot space

            faces = new Sprite3D[4];
            if (listTextures != null)
            {
                faces[0] = new Sprite3D(16.0f, 9.0f, listTextures[0]);
                faces[1] = new Sprite3D(16.0f, 9.0f, listTextures[1]);
                faces[2] = new Sprite3D(16.0f, 9.0f, listTextures[2]);
                faces[3] = new Sprite3D(16.0f, 9.0f, listTextures[3]);
            }
            else 
            {
                faces[0] = new Sprite3D(16.0f, 9.0f, null);
                faces[1] = new Sprite3D(16.0f, 9.0f, null);
                faces[2] = new Sprite3D(16.0f, 9.0f, null);
                faces[3] = new Sprite3D(16.0f, 9.0f, null);
            }

            
            //Puesto que los cuadrados se crean en la posicion 0 0 0 debo de trasladarlos y rotarlos para 
            //formar un cubo con las caras hacia dentro
            faces[0].Position = new Vector3(0.0f, 0.0f, -faces[0].Ancho / 2);

            faces[1].Rotation = Matrix.CreateRotationY(-MathHelper.PiOver2);
            faces[1].Position = new Vector3(faces[1].Ancho / 2, 0.0f, 0.0f);

            faces[2].Rotation = Matrix.CreateRotationY(MathHelper.Pi);
            faces[2].Position = new Vector3(0.0f, 0.0f, faces[2].Ancho / 2);

            faces[3].Rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            faces[3].Position = new Vector3(-faces[3].Ancho / 2, 0.0f, 0.0f);

            effect = new BasicEffect(graphicsDevice);
        }

        public virtual void Update(GameTime gameTime){
            foreach (GoTo sprite in listaGoTo)
            {
                sprite.Update(gameTime);
            }

            foreach (HotSpot sprite in listaHotSpot)
            {
                sprite.Update(gameTime);
            }
        }

        public virtual void Draw(Camera camera) {
            if (visible)
            {
                foreach (Sprite3D cuadrado in faces) {
                    cuadrado.Draw(camera);
                }

                foreach (GoTo cuadrado in listaGoTo)
                {
                    cuadrado.Draw(camera);
                }

                foreach (HotSpot cuadrdo in listaHotSpot)
                {
                    cuadrdo.Draw(camera);
                }
            }
        }

        public bool BlendingAnimationUpdate(GameTime gameTime) {
            if (initBlending != null) {
                //Calcular porcentaje de avance de la animacion
                float porcentaje = (float)((int)gameTime.TotalGameTime.TotalMilliseconds - initBlending) / (float)blendingDurationMiliseg;
                foreach (Sprite3D cuadrado in faces)
                {
                    cuadrado.SetTransitionAnimationProgress(porcentaje);
                }
                if (porcentaje >= 1.0)
                {
                    initBlending = null;
                    return true;
                }
            }
            return false;
        }
        
        public bool RotationAnimationUpdate(GameTime gameTime)
        {
            
            float radPerCall = 0.001f;
            actualRotationRad += radPerCall;
            if (totalRotationRad >= 0)
            {
                PanoEngine.PECamera.RotateLookAt(radPerCall, Camera.AXIS.Y);
            }
            else {
                PanoEngine.PECamera.RotateLookAt(-radPerCall, Camera.AXIS.Y);
            }
            //System.Diagnostics.Debug.WriteLine(totalRotationRad + " " + actualRotationRad);
            if (Math.Abs(totalRotationRad) <= actualRotationRad)
                return true;
            
            return false;
        }

        #endregion

        #region Helper methods

        public virtual Matrix GetWorld()
        {
            return world * rotation * scale * position;
        }

        public void SetListaTexturas(String[] listaTexturas) {
            faces[0].TexturePath = listaTexturas[0];
            faces[1].TexturePath = listaTexturas[1];
            faces[2].TexturePath = listaTexturas[2];
            faces[3].TexturePath = listaTexturas[3];
        }
        public Texture2D[] GetTexturesList() 
        {
            Texture2D[] l = new Texture2D[4];
            l[0] = faces[0].Texture;
            l[1] = faces[1].Texture;
            l[2] = faces[2].Texture;
            l[3] = faces[3].Texture;

            return l;
        }

        public void SetTextureToFace(HotSpace.WALL wall, String textureP)
        {
            faces[(int)wall].TexturePath = textureP;
        }

        public void LoadContent() 
        {
            foreach (Sprite3D f in faces) {
                f.LoadContent();
            }
        }

        public void UnloadContent()
        {
            foreach (Sprite3D f in faces)
            {
                f.UnloadContent();
            }
        }

        public void addHotSpot(int coordX, int coordY, WALL wall, float scale, HotSpot.ClickEventHandler handler)
        {
            listaHotSpot.Add(new HotSpot());
            
            listaHotSpot[listaHotSpot.Count - 1].ClickEvent = handler;
            listaHotSpot[listaHotSpot.Count - 1].Scale = Matrix.CreateScale(scale);
            listaHotSpot[listaHotSpot.Count - 1].Position = new Vector3((float)coordX, (float)coordY, -4.99f);
            //calcular posicion y rotacion segun la pared donde este
            float rotation = 0.0f;
            switch(wall)
            {
                case WALL.EAST:
                    rotation = MathHelper.PiOver2;
                    break;
                case WALL.NORTH:
                    rotation = 0.0f;
                    break;
                case WALL.WEST:
                    rotation = -MathHelper.PiOver2;
                    break;
                case WALL.SOUTH:
                    rotation = MathHelper.Pi;
                    break;
            }

            listaHotSpot[listaHotSpot.Count - 1].Rotation = Matrix.CreateRotationY(rotation);
        }

        public void addGoTo(int coordX, int coordY, WALL wall, float scale, string destinyName, GoTo.ClickGoToHandler handler)
        {

            listaGoTo.Add(new GoTo());

            listaGoTo[listaGoTo.Count - 1].DestinyName = destinyName;
            listaGoTo[listaGoTo.Count - 1].GoToEvent = handler;
            listaGoTo[listaGoTo.Count - 1].Scale = Matrix.CreateScale(scale);
            
            //calcular posicion y rotacion segun la pared donde este
            //mapeando las coordenadas indicadas en la textura
            float antiClippingOffset = 0.01f;
            float rotation = 0.0f;
            float x = 0.0f, y = 0.0f, z = 0.0f;

            faces[(int)wall].LoadContent();
            float imageWidth = faces[(int)wall].Texture.Width;
            float imageHeight = faces[(int)wall].Texture.Height;
            faces[(int)wall].UnloadContent();

            float wallWidth = faces[(int)wall].Ancho;
            float wallHeight = faces[(int)wall].Alto;
            float desp = faces[(int)wall].Ancho / 2;
            switch (wall)
            {
                case WALL.EAST:
                    rotation = -MathHelper.PiOver2;
                    x = desp - antiClippingOffset;
                    y = (2 * (coordY / imageHeight) - 1) * (-wallHeight / 2);
                    z = (2 * (coordX / imageWidth) - 1) * (wallWidth / 2);
                    break;
                case WALL.NORTH:
                    rotation = 0.0f;
                    x = (2 * (coordX / imageWidth) - 1) * (wallWidth / 2);
                    y = (2 * (coordY / imageHeight) - 1) * (-wallHeight / 2);
                    z = -desp + antiClippingOffset;
                    break;
                case WALL.WEST:
                    rotation = MathHelper.PiOver2;
                    x = -desp + antiClippingOffset;
                    y = (2 * (coordY / imageHeight) - 1) * (-wallHeight / 2);
                    z = (2 * (coordX / imageWidth) - 1) * (-wallWidth / 2);
                    break;
                case WALL.SOUTH:
                    rotation = MathHelper.Pi;
                    x = (2 * (coordX / imageWidth) - 1) * (-wallWidth / 2);
                    y = (2 * (coordY / imageHeight) - 1) * (-wallHeight / 2);
                    z = desp - antiClippingOffset;
                    break;
            }
            listaGoTo[listaGoTo.Count - 1].Rotation = Matrix.CreateRotationY(rotation);
            listaGoTo[listaGoTo.Count - 1].Position = new Vector3(x,y,z);
        }

        public void BeginBlendingAnimation(Texture2D[] listaTexturas, int duration, GameTime gameTime)
        {
            initBlending = (int)gameTime.TotalGameTime.TotalMilliseconds;
            blendingDurationMiliseg = duration * 1000;
            faces[0].BeginTransitionAnimation(listaTexturas[0], effect);
            faces[1].BeginTransitionAnimation(listaTexturas[1], effect);
            faces[2].BeginTransitionAnimation(listaTexturas[2], effect);
            faces[3].BeginTransitionAnimation(listaTexturas[3], effect);
        }

        public void BeginRotationAnimation(Vector2 pointer)
        {
            //calcular rotation de x e y
            int width = PanoEngine.PEAssociatedGame.Window.ClientBounds.Width; //????
            totalRotationRad = (width / 2 - pointer.X) * (PanoEngine.PECamera.FIELD_OF_VIEW / width);
            totalRotationRad = (float)Math.PI * totalRotationRad / 180.0f;
            totalRotationRad *= 1.45f;
            actualRotationRad = 0;
        }

        #endregion
    }
}
