﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaseoSurface
{
    class Resources
    {
        public Game Game {get; set;}
        public ContentManager Content { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public Camera Camera { get; set; }
        public bool MovementEnabled { get; set; }

        public bool PointerPressed { get; set; }
        public int PointerX { get; set; }
        public int PointerY { get; set; }



        #region Singleton Implementation
        private static Resources instance;
        public static Resources Instance 
        {
            get 
            {
                if (instance == null) 
                {
                    instance = new Resources();
                }

                return instance;
            }
        }

        private Resources() { }

        #endregion

    }
}