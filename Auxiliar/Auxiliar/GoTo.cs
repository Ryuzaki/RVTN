//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Content;

//namespace Auxiliar
//{
//    public class GoTo : Cuadrado3D
//    {
//        #region Fields
//        private ClickGoToHandler goToEvent;

//        private bool wasPressed = false;

//        private String destinyName;

//        #endregion

//        #region Properties
//        public ClickGoToHandler GoToEvent
//        {
//            get {
//                return goToEvent;
//            }
//            set {
//                goToEvent = value;
//            }
//        }

//        public String DestinyName
//        {
//            get {
//                return destinyName;
//            }
//            set {
//                destinyName = value;
//            }
//        }

//        #endregion

//        #region Contructor, Draw and Update


//        public GoTo(GraphicsDevice graphicsDevice, ContentManager content):base(content.Load<Texture2D>(@"Assets\GoTo"), graphicsDevice)
//        {

//        }

//        public override void Update(GameTime gameTime, Camera camera)
//        {
//            base.Update(gameTime, camera);

//            /*Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
//            Viewport viewport = this.graphicsDevice.Viewport;
//            if (InteractiveHelper.Intersects(mouseLocation, this, camera, viewport) && goToEvent != null)
//            {
//                if (wasPressed == true && Mouse.GetState().LeftButton == ButtonState.Released) {
//                    goToEvent(destinyName); //Llamar al metodo que cambia de HotSpace
//                    wasPressed = false;
//                }
//                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
//                    wasPressed = true;
//           }*/
//        }

//        #endregion

//        #region Helper methods
//        public delegate void ClickGoToHandler(String destiny);

//        #endregion
//    }
//}
