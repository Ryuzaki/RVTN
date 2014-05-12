using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaseoSurface
{
    public class HotSpot : Sprite3D
    {
        #region Fields
        private ClickEventHandler clickEvent;

        private bool wasPressed = false;

        #endregion

        #region Properties
        public ClickEventHandler ClickEvent
        {
            get {
                return clickEvent;
            }
            set {
                clickEvent = value;
            }
        }
        #endregion

        #region Contructor, Draw and Update


        public HotSpot():base(1.0f,1.0f,@"Assets\HotSpot")
        {
            LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region Helper methods
        public delegate void ClickEventHandler(GameTime gameTime);

        #endregion
    }
}
