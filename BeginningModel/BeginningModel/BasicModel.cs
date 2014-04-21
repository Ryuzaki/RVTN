using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BeginningModel
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;

        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(5);

        public BasicModel(Model m) {
            model = m;
        }

        public virtual void Update() {
            rotation *= Matrix.CreateRotationZ(MathHelper.Pi / 180);

        }

        public void Draw(ModelManager manager, Camera camera) { 
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            
            
            

            foreach (ModelMesh mesh in model.Meshes) {

                
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = manager.effect;              
                    manager.effect.Parameters["World"].SetValue(GetWorld() * mesh.ParentBone.Transform);
                    manager.effect.Parameters["View"].SetValue(camera.view);
                    manager.effect.Parameters["Projection"].SetValue(camera.projection);
                }
                mesh.Draw();
            }

        }

        public virtual Matrix GetWorld() {
            return world * rotation * scale;
        }
    }
}
