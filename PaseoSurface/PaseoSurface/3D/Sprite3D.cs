using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace PaseoSurface
{
    public class Sprite3D
    {
        #region Fields

        protected String _texturePath;
        protected Texture2D _texture;
        protected VertexPositionTexture[] _verts;
        protected VertexBuffer _vertexBuffer;
        protected Matrix _world = Matrix.Identity;

        protected Matrix _rotation = Matrix.Identity;
        protected Matrix _scale = Matrix.CreateScale(1);
        protected Matrix _position = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        protected bool _visible = true;
        protected float _ancho;
        protected float _alto;

        //Transition animation fields
        protected BasicEffect basicEffect;
        protected Effect TAEffect;
        protected Texture2D destinyTexture = null;
        protected float animationProgress = 0;
        protected bool IsTransitionAnimationInProgress = false;



        #endregion

        #region Properties
        public float Ancho 
        {
            get 
            {
                return _ancho;
            }
        }
        public float Alto 
        {
            get 
            {
                return _alto;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
            }
        }
        public String TexturePath
        {
            get 
            {
                return _texturePath; 
            }
            set 
            { 
                _texturePath = value; 
            }
        }

        public Matrix World
        {
            get
            {
                return _world;
            }
            set
            {
                _world = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return _position.Translation;
            }
            set
            {
                _position = Matrix.CreateTranslation(value);
            }
        }

        public Matrix Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                //_ancho = 2 * value.M11;
                //_alto = 2 * value.M22;
                _scale = value;
            }
        }

        public Matrix Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        #endregion

        #region Constructor, Draw and Update

        public Sprite3D(GraphicsDevice g, ContentManager c, float ancho, float alto)
        {
            ConstructorSprite3D(ancho, alto, null);
        }

        public Sprite3D(float ancho, float alto, String textureP)
        {
            ConstructorSprite3D(ancho, alto, textureP);
        }

        private void ConstructorSprite3D(float ancho, float alto, String textureP)
        {
            _ancho = ancho;
            _alto = alto;
            _scale = Matrix.CreateScale(_ancho / 2, _alto / 2, 1.0f);
            //_graphicsDevice = PanoEngine.PEGraphicsDevice;
            _texturePath = textureP;
            GraphicsDevice gd = Resources.Instance.GraphicsDevice;
            basicEffect = new BasicEffect(gd);
            //Crear el cuadrado a partir de 4 vertices
            _verts = new VertexPositionTexture[4];
            //Aunque se cree cuadrado se cambiara al ser multiplicado por su matriz de escalado
            _verts[0] = new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 0), new Vector2(0, 0));
            _verts[1] = new VertexPositionTexture(new Vector3(1.0f, 1.0f, 0), new Vector2(1, 0));
            _verts[2] = new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 0), new Vector2(0, 1));
            _verts[3] = new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0), new Vector2(1, 1));
            _vertexBuffer = new VertexBuffer(Resources.Instance.GraphicsDevice, typeof(VertexPositionTexture), _verts.Length, BufferUsage.None);
            _vertexBuffer.SetData(_verts);
        }

        public virtual void Update(GameTime gameTime){

        }

        public virtual void Draw(Camera camera) {
            if (_visible)
            {
                foreach (EffectPass pass in GetEffectToUse(camera).CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Resources.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    Resources.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, _verts, 0, 2);
                    Resources.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                }
                //BoundingSphereRenderer.Render(GetBoundingSphere(), Resources.Instance.GraphicsDevice, camera.View, camera.Projection, Color.Red);
            }
        }


        #endregion

        #region Helper methods

        public void LoadContent() 
        {
            if (_texturePath != null) 
            {
                String exePath = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location);
                String contentPath = exePath + "\\" + Resources.Instance.Content.RootDirectory + "\\" + _texturePath + ".xnb";
                String fsPath = exePath + "\\" + _texturePath;
                if(File.Exists(contentPath))
                {
                    _texture = Resources.Instance.Content.Load<Texture2D>(_texturePath);
                }else
                {
                    using (FileStream fileStream = new FileStream(@fsPath, FileMode.Open))
                    {
                        _texture = Texture2D.FromStream(Resources.Instance.GraphicsDevice, fileStream);
                    }
                }
            }
        }

        public void UnloadContent() 
        {
            _texture = null;
        }

        public virtual Matrix GetWorld()
        {
            return _world * _scale * _rotation * _position;
        }

        public BoundingSphere GetBoundingSphere()
        {
            BoundingSphere sphere = new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            sphere = sphere.Transform(GetWorld());
            return sphere;
        }

        public Effect GetEffectToUse(Camera camera) {

            if (!IsTransitionAnimationInProgress)
            {
                basicEffect.World = GetWorld();
                basicEffect.View = camera.View;
                basicEffect.Projection = camera.Projection;
                basicEffect.Texture = _texture;
                basicEffect.TextureEnabled = true;

                return basicEffect;
            }
            else {
                TAEffect.Parameters["World"].SetValue(GetWorld());
                TAEffect.Parameters["View"].SetValue(camera.View);
                TAEffect.Parameters["Projection"].SetValue(camera.Projection);
                TAEffect.Parameters["TextureOrigin"].SetValue(_texture);
                TAEffect.Parameters["TextureDestiny"].SetValue(destinyTexture);
                TAEffect.Parameters["AlphaPercentaje"].SetValue(animationProgress);


                return TAEffect;
            }
        }

        public void BeginTransitionAnimation(Texture2D texture, Effect effect) {
            destinyTexture = texture;
            IsTransitionAnimationInProgress = true;
            TAEffect = Resources.Instance.Content.Load<Effect>(@"Effects/TransEffect");
        }

        public void SetTransitionAnimationProgress(float progress) {
            animationProgress = progress;
            if (animationProgress >= 1.0)
            {
                IsTransitionAnimationInProgress = false;
                _texture = destinyTexture;
            }

        }

        #endregion

        
    }
}
