using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaseoSurface
{
    public class GoTo : Sprite3D
    {
        #region Fields
        private ClickGoToHandler goToEvent;

        private bool wasPressed = false;

        private String destinyName;

        #endregion

        #region Properties
        public ClickGoToHandler GoToEvent
        {
            get {
                return goToEvent;
            }
            set {
                goToEvent = value;
            }
        }

        public String DestinyName
        {
            get {
                return destinyName;
            }
            set {
                destinyName = value;
            }
        }

        #endregion

        #region Contructor, Draw and Update


        public GoTo():base(1.0f,1.0f,@"Assets\GoTo")
        {
            LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Comprobar 
            Vector2 pointerLocation = new Vector2(Resources.Instance.PointerX, Resources.Instance.PointerY);
            Viewport viewport = Resources.Instance.GraphicsDevice.Viewport;
            Camera camera = Resources.Instance.Camera;
            if (Resources.Instance.PointerPressed == true && 
                goToEvent != null && 
                Camera.Intersects(this, pointerLocation, viewport, camera))
            {
                //PanoEngine.start = new TimeSpan(DateTime.Now.Ticks);
                goToEvent(pointerLocation, destinyName, gameTime); //Llamar al metodo que cambia de HotSpace
            }
            


        }

        #endregion

        #region Helper methods
        public delegate void ClickGoToHandler(Vector2 pointer, String destiny, GameTime gameTime);

        #endregion
    }
}
