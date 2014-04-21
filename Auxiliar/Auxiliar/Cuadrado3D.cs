using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliar
{
    public class Cuadrado3D
    {
        #region Fields

        protected GraphicsDevice graphicsDevice;
        protected Texture2D texture;
        protected VertexPositionTexture[] verts;
        //protected VertexPositionColor[] verts;
        protected VertexBuffer vertexBuffer;
        protected Matrix world = Matrix.Identity;

        protected Matrix rotation = Matrix.Identity;
        protected Matrix scale = Matrix.CreateScale(1);
        protected Matrix position = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        protected bool visible = true;

        //Transition animation fields
        protected BasicEffect basicEffect;
        protected Effect TAEffect;
        protected Texture2D destinyTexture = null;
        protected float animationProgress = 0;
        protected bool IsTransitionAnimationInProgress = false;



        #endregion

        #region Properties

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }

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

        #endregion

        #region Constructor, Draw and Update

        public Cuadrado3D(GraphicsDevice graphicsDevice)
        {
            ConstructorCuadrado3D(graphicsDevice, null);
        }

        public Cuadrado3D(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            ConstructorCuadrado3D(graphicsDevice, texture);
        }

        private void ConstructorCuadrado3D(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            this.graphicsDevice = graphicsDevice;
            this.texture = texture;
            basicEffect = new BasicEffect(graphicsDevice);
            //Crear el cuadrado a partir de 4 vertices
            verts = new VertexPositionTexture[4];
            verts[0] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
            verts[2] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1));
            verts[3] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1));
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);
        }

        public virtual void Update(GameTime gameTime, Camera camera){
        
        }

        public virtual void Draw(Camera camera) {
            if (visible)
            {


                foreach (EffectPass pass in GetEffectToUse(camera).CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.BlendState = BlendState.AlphaBlend;
                    //graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, verts, 0, 2);
                    graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, verts, 0, 2);
                    graphicsDevice.BlendState = BlendState.Opaque;
                }
            }
        }


        #endregion

        #region Helper methods

        public virtual Matrix GetWorld()
        {
            return world * rotation * scale * position;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
        }

        public Effect GetEffectToUse(Camera camera) {

            if (!IsTransitionAnimationInProgress)
            {
                basicEffect.World = GetWorld();
                basicEffect.View = camera.View;
                basicEffect.Projection = camera.Projection;
                //basicEffect.VertexColorEnabled = true;
                basicEffect.Texture = texture;
                basicEffect.TextureEnabled = true;

                return basicEffect;
            }
            else {
                TAEffect.Parameters["World"].SetValue(GetWorld());
                TAEffect.Parameters["View"].SetValue(camera.View);
                TAEffect.Parameters["Projection"].SetValue(camera.Projection);
                TAEffect.Parameters["TextureOrigin"].SetValue(texture);
                TAEffect.Parameters["TextureDestiny"].SetValue(destinyTexture);
                TAEffect.Parameters["AlphaPercentaje"].SetValue(animationProgress);


                return TAEffect;
            }
        }

       /* public void BeginTransitionAnimation(Texture2D texture, Effect effect) {
            destinyTexture = texture;
            IsTransitionAnimationInProgress = true;
            TAEffect = effect;
        }

        public void SetTransitionAnimationProgress(float progress) {
            animationProgress = progress;
            if (animationProgress >= 1.0)
            {
                IsTransitionAnimationInProgress = false;
                texture = destinyTexture;
            }

        }
        */
        #endregion

        
    }
}
