using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PanoEngine
{
    public class PaseoVirtual
    {

        #region Fields
        private enum STATE { NORMAL, TRANSITION, BLENDING };
        private STATE state = STATE.NORMAL;
        private String name;
        private List<HotSpace> listaHotSpace; //mal
        private int currentHotSpace, nextHotSpace;
        private int blendingDurationSeg = 3; //duracion de la transicion en segundos
        private int rotationDurationSeg = 2; //duracion de la transicion de giro en segundos
        #endregion

        #region Properties
        public String Name
        {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        #endregion

        #region Constructor, Draw and Update

        public PaseoVirtual() {
            listaHotSpace = new List<HotSpace>();
            currentHotSpace = 0;
        }

        public virtual void Update(GameTime gameTime) {

            switch (state) { 
                case STATE.NORMAL:
                    
                    break;
                case STATE.TRANSITION:
                    if (listaHotSpace[currentHotSpace].RotationAnimationUpdate(gameTime))
                    {
                        state = STATE.BLENDING;
                        PanoEngine.MovementEnabled = true; //Habilitar el movimiento de la camara al acabar la animacion de transicion
                        listaHotSpace[currentHotSpace].BeginBlendingAnimation(
                        listaHotSpace[nextHotSpace].GetTexturesList(), blendingDurationSeg, gameTime);
                    }

                    break;
                case STATE.BLENDING:
                    if (listaHotSpace[currentHotSpace].BlendingAnimationUpdate(gameTime))
                    {
                        listaHotSpace[currentHotSpace].UnloadContent();
                        currentHotSpace = nextHotSpace;
                        state = STATE.NORMAL;
                    }
                    break;
                default: break;
            }

            listaHotSpace[currentHotSpace].Update(gameTime);
        }

        public virtual void Draw() {
            listaHotSpace[currentHotSpace].Draw(PanoEngine.PECamera);
        }

        #endregion

        #region Functionality Methods

        public void LoadContentCurrentHotSpace() {
            //cargar contenedor actual
            listaHotSpace[currentHotSpace].LoadContent();
        }

        public void LoadAllContent()
        {
            foreach (HotSpace hs in listaHotSpace)
            {
                hs.LoadContent();
            }
        }

        public int addHotSpace(string name){
            listaHotSpace.Add(new HotSpace(name));
            
            return listaHotSpace.Count - 1;
        }

        public void addTextureToHotSpace(int index, String textureP, HotSpace.WALL wall) {
            listaHotSpace[index].SetTextureToFace(wall, textureP);
        }

        public void addHotSpotToSpace(int index, int coordX, int coordY, HotSpace.WALL wall, HotSpot.ClickEventHandler handler) {
            listaHotSpace[index].addHotSpot(coordX, coordY, wall, 1.0f, handler);
        }

        public void addGoToToSpace(int index, int coordX, int coordY, HotSpace.WALL wall, string destinyName) {
            listaHotSpace[index].addGoTo(coordX, coordY, wall, 0.2f, destinyName, new GoTo.ClickGoToHandler(GoToClickHandler));
        }

        private void GoToClickHandler(Vector2 pointer, String destinyName, GameTime gameTime)
        {
            PanoEngine.stop = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine("Inicio de GoToClickHandler: " + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
            for (int i = 0; i < listaHotSpace.Count; ++i)
            {
                if (listaHotSpace[i].Name.CompareTo(destinyName) == 0)
                {
                    state = STATE.TRANSITION;
                    PanoEngine.MovementEnabled = false; //Deshabilitar el movimiento de la camara mientras se hace la animacion de transicion
                    nextHotSpace = i;
                    PanoEngine.stop = new TimeSpan(DateTime.Now.Ticks);
                    Console.WriteLine("Antes de cargar LoadContent del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    listaHotSpace[nextHotSpace].LoadContent();
                    PanoEngine.stop = new TimeSpan(DateTime.Now.Ticks);
                    Console.WriteLine("Despues de cargar LoadContent del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    listaHotSpace[currentHotSpace].BeginRotationAnimation(pointer);
                    PanoEngine.stop = new TimeSpan(DateTime.Now.Ticks);
                    Console.WriteLine("Despues de iniciar animacion de rotacion del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    break;
                }
                
            }
        }

        #endregion


    }
}
