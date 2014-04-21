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
//    public class HotSpot : Cuadrado3D
//    {
//        #region Fields
//        private ClickEventHandler clickEvent;

//        private bool wasPressed = false;

//        #endregion

//        #region Properties
//        public ClickEventHandler ClickEvent
//        {
//            get {
//                return clickEvent;
//            }
//            set {
//                clickEvent = value;
//            }
//        }
//        #endregion

//        #region Contructor, Draw and Update


//        public HotSpot(GraphicsDevice graphicsDevice, ContentManager content):base(content.Load<Texture2D>(@"Assets\HotSpot"), graphicsDevice)
//        {

//        }

//        public override void Update(GameTime gameTime, Camera camera)
//        {
//            base.Update(gameTime, camera);

//            /*Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
//            Viewport viewport = this.graphicsDevice.Viewport;
//            if (InteractiveHelper.Intersects(mouseLocation, this, camera, viewport) && clickEvent != null){
//                if (wasPressed == true && Mouse.GetState().LeftButton == ButtonState.Released) {
//                    clickEvent(gameTime);
//                    wasPressed = false;
//                }
//                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
//                    wasPressed = true;
                
                
//           }*/
//        }

//        #endregion

//        #region Helper methods
//        public delegate void ClickEventHandler(GameTime gameTime);

//        #endregion
//    }
//}
