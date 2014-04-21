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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields
        public float FIELD_OF_VIEW = 40.0f;
        private Vector3 position;
        private Vector3 lookAt;
        private Vector3 up = Vector3.Up;
        private Matrix view;
        private Matrix projection;

        /// <summary>
        /// Yaw (guiñada) : rotacion alrededor del ejeY
        /// Pitch (cabeceo) : rotacion alrededor del eje X
        /// Roll (balanceo) : rotacion alrededor del eje Z
        /// </summary>
        private float yaw = 0, pitch = 0, roll = 0;

        #endregion

        #region Properties
        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
                SetView();
            }
        }

        public Vector3 LookAt {
            get {
                return lookAt;
            }
            set {
                lookAt = LookAt;
                SetView();
            }
        }

        public Vector3 Up {
            get {
                return up;
            }
            set {
                up = value;
                SetView();
            }
        }

        public Matrix View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        public Matrix Projection {
            get {
                return projection;
            }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="pos">Posicion de la camara.</param>
        /// <param name="target">Objetivo de la camara.</param>
        /// <param name="up">Vector normal de la camara.</param>
        public Camera(Vector3 pos, Vector3 target, Vector3 up)
            : base(PanoEngine.PEAssociatedGame)
        {
            // TODO: Construct any child components here
            this.position = pos;
            this.lookAt = target;
            this.up = up;
            //crear view y projection a partir de estos datos
            SetView();
            SetProjection();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        #region Helper Methods
        /// <summary>
        /// Crear la view matrix a partir de la posicion, lookAt y vector up.
        /// </summary>
        private void SetView() {
            view = Matrix.CreateLookAt(this.position, this.lookAt, this.up);
        }
        /// <summary>
        /// Crear la matriz de proyeccion de 90 grados.
        /// </summary>
        public void SetProjection() {
            float fov = (FIELD_OF_VIEW * MathHelper.Pi) / 180.0f;            
            projection = Matrix.CreatePerspectiveFieldOfView(fov, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 0.1f, 3000);
            //projection = Matrix.CreatePerspective(0.14f, 0.08f, 0.1f, 3000); //HFOV >> VFOV 
            
        }

        public enum AXIS {X, Y, Z};
        /// <summary>
        /// Rotar la camara en un eje.
        /// </summary>
        /// <param name="rad">Angulo de giro en radianes.</param>
        /// <param name="axis">Eje de rotacion.</param>
        public void RotateLookAt(float rad, AXIS axis) {

            Vector3 nuevoEje = Vector3.Zero;
            switch (axis)
            {
                case AXIS.X:
                    pitch += rad;
                    break;
                case AXIS.Y:
                    yaw += rad;
                    break;
                case AXIS.Z:
                    roll += rad;
                    break;
            }
            Vector3 cameraReference = new Vector3(0, 0, -1);
            Matrix rotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw) * Matrix.CreateRotationZ(roll);
            Vector3 referenciaTransformada = Vector3.Transform(cameraReference, rotation);
            this.lookAt = position + referenciaTransformada;
            SetView();
        }

        public static bool Intersects(Sprite3D s, Vector2 pointer, Viewport v, Camera cam) 
        {
            Vector3 nearPoint = new Vector3(pointer, 0);
            Vector3 farPoint = new Vector3(pointer, 1);

            nearPoint = v.Unproject(nearPoint, cam.Projection, cam.View, Matrix.Identity);
            farPoint = v.Unproject(farPoint, cam.Projection, cam.View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray r = new Ray(nearPoint, direction);

            BoundingSphere bs = s.GetBoundingSphere();

            return (bs.Intersects(r) != null) ? true : false;
        }

        #endregion

    }
}
