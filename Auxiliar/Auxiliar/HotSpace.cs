using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliar
{
    public class HotSpace
    {
        #region Fields

        private GraphicsDevice graphicsDevice;
        private Cuadrado3D[] faces;
        private BasicEffect effect;

        private Matrix world = Matrix.Identity;
        private Matrix rotation = Matrix.Identity;
        private Matrix scale = Matrix.CreateScale(1);
        private Matrix position = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private bool visible = true;

        //Transition animation fields
        private bool IsTAOnline = false;
        private int TABegin = 0;
        private int TADurationMiliseg = 0;

        private String name;
        public enum WALL { NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3 };
        //private List<HotSpot> listaHotSpot;        
        //private List<GoTo> listaGoTo;

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
                foreach (Cuadrado3D cuadrado in faces)
                {
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
                foreach (Cuadrado3D cuadrado in faces)
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
        public HotSpace(GraphicsDevice graphicsDevice, String name, Texture2D[] listTextures, bool ext)
        {
            this.graphicsDevice = graphicsDevice;
            this.name = name;

            //inicializar lista de hotspots
            //listaHotSpot = new List<HotSpot>();

            //inicializar lista de gotos
            //listaGoTo = new List<GoTo>();

            //Inicializar caras del hot space

            faces = new Cuadrado3D[4];
            faces[0] = new Cuadrado3D(graphicsDevice, listTextures[0]);
            faces[1] = new Cuadrado3D(graphicsDevice, listTextures[1]);
            faces[2] = new Cuadrado3D(graphicsDevice, listTextures[2]);
            faces[3] = new Cuadrado3D(graphicsDevice, listTextures[3]);
            //faces[4] = new Cuadrado3D(listTextures[4]);
            //faces[5] = new Cuadrado3D(listTextures[5]);

            if (ext) faces[0].Rotation = Matrix.CreateRotationY(MathHelper.Pi);
            faces[0].Position = new Vector3(0.0f, 0.0f, -1.0f);

            if (ext) faces[1].Rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            else faces[1].Rotation = Matrix.CreateRotationY(-MathHelper.PiOver2);
            faces[1].Position = new Vector3(1.0f, 0.0f, 0.0f);

            if (!ext) faces[2].Rotation = Matrix.CreateRotationY(MathHelper.Pi);
            faces[2].Position = new Vector3(0.0f, 0.0f, 1.0f);

            if (ext) faces[3].Rotation = Matrix.CreateRotationY(-MathHelper.PiOver2);
            else faces[3].Rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            faces[3].Position = new Vector3(-1.0f, 0.0f, 0.0f);

            /*if (ext) faces[4].Rotation = Matrix.CreateRotationX(-MathHelper.PiOver2);
            else faces[4].Rotation = Matrix.CreateRotationX(MathHelper.PiOver2);
            faces[4].Position = new Vector3(0.0f, 1.0f, 0.0f);

            if (ext) faces[5].Rotation = Matrix.CreateRotationX(MathHelper.PiOver2);
            else faces[5].Rotation = Matrix.CreateRotationX(-MathHelper.PiOver2);
            faces[5].Position = new Vector3(0.0f, -1.0f, 0.0f);*/


            effect = new BasicEffect(graphicsDevice);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotSpace"/> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="listTextures">Lista de Texture2D correspondiente al frontal, derecha, atras, izquierda, arriba y abajo
        /// respectivamente. </param>
        /*public HotSpace(String name, bool ext)
        {
            this.graphicsDevice = PanoEngine.PEGraphicsDevice;
            this.name = name;

            //inicializar lista de hotspots
            listaHotSpot = new List<HotSpot>();

            //inicializar lista de gotos
            listaGoTo = new List<GoTo>();

            //Inicializar caras del hot space

            faces = new Cuadrado3D[4];
            faces[0] = new Cuadrado3D();
            faces[1] = new Cuadrado3D();
            faces[2] = new Cuadrado3D();
            faces[3] = new Cuadrado3D();
            //faces[4] = new Cuadrado3D(listTextures[4]);
            //faces[5] = new Cuadrado3D(listTextures[5]);

            if (ext) faces[0].Rotation = Matrix.CreateRotationY(MathHelper.Pi);
            faces[0].Position = new Vector3(0.0f, 0.0f, -1.0f);

            if (ext) faces[1].Rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            else faces[1].Rotation = Matrix.CreateRotationY(-MathHelper.PiOver2);
            faces[1].Position = new Vector3(1.0f, 0.0f, 0.0f);

            if (!ext) faces[2].Rotation = Matrix.CreateRotationY(MathHelper.Pi);
            faces[2].Position = new Vector3(0.0f, 0.0f, 1.0f);

            if (ext) faces[3].Rotation = Matrix.CreateRotationY(-MathHelper.PiOver2);
            else faces[3].Rotation = Matrix.CreateRotationY(MathHelper.PiOver2);
            faces[3].Position = new Vector3(-1.0f, 0.0f, 0.0f);

            /*if (ext) faces[4].Rotation = Matrix.CreateRotationX(-MathHelper.PiOver2);
            else faces[4].Rotation = Matrix.CreateRotationX(MathHelper.PiOver2);
            faces[4].Position = new Vector3(0.0f, 1.0f, 0.0f);

            if (ext) faces[5].Rotation = Matrix.CreateRotationX(MathHelper.PiOver2);
            else faces[5].Rotation = Matrix.CreateRotationX(-MathHelper.PiOver2);
            faces[5].Position = new Vector3(0.0f, -1.0f, 0.0f);*/

        /*
            effect = new BasicEffect(graphicsDevice);

        }*/

        public virtual void Update(GameTime gameTime)
        {
            TransitionAnimationUpdate(gameTime);
        }

        public virtual void Draw(Camera camera)
        {
            if (visible)
            {
                foreach (Cuadrado3D cuadrado in faces)
                {
                    cuadrado.Draw(camera);
                }

                /*foreach (GoTo cuadrado in listaGoTo)
                {
                    cuadrado.Draw(camera);
                }

                foreach (HotSpot cuadrdo in listaHotSpot)
                {
                    cuadrdo.Draw(camera);
                }*/
            }
        }

        private void TransitionAnimationUpdate(GameTime gameTime)
        {
            /*if (IsTAOnline)
            {
                System.Diagnostics.Debug.WriteLine("Animacion en progreso");
                //Calcular porcentaje de avance de la animacion
                float porcentaje = (float)((int)gameTime.TotalGameTime.TotalMilliseconds - TABegin) / (float)TADurationMiliseg;

                foreach (Cuadrado3D cuadrado in faces)
                {
                    cuadrado.SetTransitionAnimationProgress(porcentaje);
                }
                if (porcentaje >= 1.0)
                {
                    IsTAOnline = false;
                }
            }*/
        }

        #endregion

        #region Helper methods

        public virtual Matrix GetWorld()
        {
            return world * rotation * scale * position;
        }

        public void SetListaTexturas(Texture2D[] listaTexturas)
        {
            faces[0].Texture = listaTexturas[0];
            faces[1].Texture = listaTexturas[1];
            faces[2].Texture = listaTexturas[2];
            faces[3].Texture = listaTexturas[3];
            //faces[4].Texture = listaTexturas[4];
            //faces[5].Texture = listaTexturas[5];
        }

        public void SetTextureToFace(HotSpace.WALL wall, Texture2D texture)
        {
            faces[(int)wall].Texture = texture;
        }


        //public void addHotSpot(int coordX, int coordY, WALL wall, float scale, HotSpot.ClickEventHandler handler)
        //{
        //    listaHotSpot.Add(new HotSpot());
            
        //    listaHotSpot[listaHotSpot.Count - 1].ClickEvent = handler;
        //    listaHotSpot[listaHotSpot.Count - 1].Scale = Matrix.CreateScale(scale);
        //    listaHotSpot[listaHotSpot.Count - 1].Position = new Vector3((float)coordX, (float)coordY, -4.99f);
        //    //calcular posicion y rotacion segun la pared donde este
        //    float rotation = 0.0f;
        //    switch(wall)
        //    {
        //        case WALL.EAST:
        //            rotation = MathHelper.PiOver2;
        //            break;
        //        case WALL.NORTH:
        //            rotation = 0.0f;
        //            break;
        //        case WALL.WEST:
        //            rotation = -MathHelper.PiOver2;
        //            break;
        //        case WALL.SOUTH:
        //            rotation = MathHelper.Pi;
        //            break;
        //    }

        //    listaHotSpot[listaHotSpot.Count - 1].Rotation = Matrix.CreateRotationY(rotation);
        //}

        //public void addGoTo(int coordX, int coordY, WALL wall, float scale, string destinyName, GoTo.ClickGoToHandler handler)
        //{
        //    listaGoTo.Add(new GoTo());

        //    listaGoTo[listaGoTo.Count - 1].DestinyName = destinyName;
        //    listaGoTo[listaGoTo.Count - 1].GoToEvent = handler;
        //    listaGoTo[listaGoTo.Count - 1].Scale = Matrix.CreateScale(scale);
        //    listaGoTo[listaGoTo.Count - 1].Position = new Vector3((float)coordX, (float)coordY, -4.99f);
        //    calcular posicion y rotacion segun la pared donde este
        //    float rotation = 0.0f;
        //    switch (wall)
        //    {
        //        case WALL.EAST:
        //            rotation = MathHelper.PiOver2;
        //            break;
        //        case WALL.NORTH:
        //            rotation = 0.0f;
        //            break;
        //        case WALL.WEST:
        //            rotation = -MathHelper.PiOver2;
        //            break;
        //        case WALL.SOUTH:
        //            rotation = MathHelper.Pi;
        //            break;
        //    }

        //    listaGoTo[listaGoTo.Count - 1].Rotation = Matrix.CreateRotationY(rotation);
        //}

        //public void BeginAnimation(Texture2D[] listaTexturas, int seg, GameTime gameTime, Effect effect)
        //{
        //    TABegin = (int)gameTime.TotalGameTime.TotalMilliseconds;
        //    IsTAOnline = true;
        //    TADurationMiliseg = seg * 1000;

        //    faces[0].BeginTransitionAnimation(listaTexturas[0], effect);
        //    faces[1].BeginTransitionAnimation(listaTexturas[1], effect);
        //    faces[2].BeginTransitionAnimation(listaTexturas[2], effect);
        //    faces[3].BeginTransitionAnimation(listaTexturas[3], effect);
        //    //faces[4].BeginTransitionAnimation(listaTexturas[4], effect);
        //    //faces[5].BeginTransitionAnimation(listaTexturas[5], effect);



        //}

        #endregion
    }
}
