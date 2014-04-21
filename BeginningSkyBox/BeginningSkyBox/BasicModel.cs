using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BeginningSkyBox
{
    /*class BasicModel
    {
        #region Fields
        private Model model;
        private Matrix world = Matrix.Identity;

        private Matrix rotation = Matrix.Identity;
        private Matrix scale = Matrix.CreateScale(1);
        private Matrix position = Matrix.CreateTranslation(new Vector3(0,0,3));
        #endregion

        #region Properties
        public Model Model {
            get {
                return model;
            }
            set {
                model = value;
            }
        }

        public Matrix World {
            get {
                return world;
            }
            set {
                world = value;
            }
        }

        public Matrix Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
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

        #endregion

        public BasicModel(Model m) {
            model = m;
        }

        public virtual void Update() {
        }

        public void Draw(Camera camera) { 
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect be in mesh.Effects) {
                    be.EnableDefaultLighting();
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }

        }

        public virtual Matrix GetWorld() {
            return world * rotation * scale * position;
        }
    }*/
}
