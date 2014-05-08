using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace PanoEngine
{
    public class PaseoVirtual
    {

        #region Fields

        private static PaseoVirtual _instance;

        private Game _associatedGame;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private Camera _camera;

        private bool _isPaseoVirtualCreated;
        private int _pointerX;
        private int _pointerY;
        private bool _pointerPressed;
        private bool _pointerReleased;
        private bool _paseoCreated;
        private bool _movementEnabled = true;

        public static TimeSpan stop;
        public static TimeSpan start;

        private static Dictionary<HotSpace.WALL, String> mapWallString = new Dictionary<HotSpace.WALL, string>()
        {
            {HotSpace.WALL.EAST, "East"},
            {HotSpace.WALL.NORTH, "North"},
            {HotSpace.WALL.SOUTH, "South"},
            {HotSpace.WALL.WEST, "West"}
        };


        private enum STATE { NORMAL, TRANSITION, BLENDING };
        private STATE state = STATE.NORMAL;
        private String name;
        private List<HotSpace> listaHotSpace; //mal
        private int currentHotSpace, nextHotSpace;
        private int blendingDurationSeg = 3; //duracion de la transicion en segundos
        private int rotationDurationSeg = 2; //duracion de la transicion de giro en segundos



        #endregion

        #region Properties
        public PaseoVirtual Instance 
        {
            get {
                if (_instance == null)
                    _instance = new PaseoVirtual();
                return _instance;
            }
        }
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return _graphicsDevice;
            }
        }
        public ContentManager Content
        {
            get
            {
                return _content;
            }
        }
        public Camera Camera
        {
            get
            {
                return _camera;
            }
        }
        public bool IsPaseoVirtualCreated
        {
            get
            {
                return _isPaseoVirtualCreated;
            }
        }
        

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
        }

        public void Initialize(Game game)
        {
            _associatedGame = game;
            _graphicsDevice = game.GraphicsDevice;
            _content = new ContentManager(game.Content.ServiceProvider, "Content");
            _camera = new Camera(Vector3.Zero, new Vector3(0, 0, -1), Vector3.Up);
            game.Components.Add(_camera);

            _pointerX = 0;
            _pointerY = 0;
            _pointerPressed = false;
            _pointerReleased = false;
            _paseoCreated = false;
            _isPaseoVirtualCreated = false;
            listaHotSpace = new List<HotSpace>();
            currentHotSpace = 0;
        }

        public void Create(string pvfile)
        {
            System.Diagnostics.Debug.WriteLine("Leyendo paseo virtual...");
            Task task = new Task(() => CreatePaseo(pvfile));
            task.ContinueWith((t) => CreatePaseoFinished());
            task.Start();
            System.Diagnostics.Debug.WriteLine("Paseo virtual leyendose...");
        }
        private void CreatePaseo(String pvfile)
        {
            using (FileStream fileStream = File.OpenRead(pvfile))
            {
                XmlReaderSettings settings;
                settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                using (XmlReader reader = XmlReader.Create(fileStream, settings))
                {
                    //Nombre del paseo
                    reader.Read();
                    reader.MoveToNextAttribute();
                    Name = reader.Value;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "HotSpace")
                        {
                            if (reader.IsStartElement())
                            {
                                //<HotSpace>
                                procesarHotSpace(reader);
                            }
                        }
                    }
                }
                LoadContentCurrentHotSpace();
            }
            System.Diagnostics.Debug.WriteLine("Create Paseo Finished v1");
        }

        private void CreatePaseoFinished()
        {
            System.Diagnostics.Debug.WriteLine("Create Paseo Finished v2");
            _isPaseoVirtualCreated = true;
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
            listaHotSpace[currentHotSpace].Draw(_camera);
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

        private void GoToClickHandler(Vector2 pointer, String destinyName, GameTime gameTime)
        {
            stop = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine("Inicio de GoToClickHandler: " + stop.Subtract(start).TotalMilliseconds);
            for (int i = 0; i < listaHotSpace.Count; ++i)
            {
                if (listaHotSpace[i].Name.CompareTo(destinyName) == 0)
                {
                    state = STATE.TRANSITION;
                    _movementEnabled = false; //Deshabilitar el movimiento de la camara mientras se hace la animacion de transicion
                    nextHotSpace = i;
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    //Console.WriteLine("Antes de cargar LoadContent del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    listaHotSpace[nextHotSpace].LoadContent();
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    //Console.WriteLine("Despues de cargar LoadContent del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    listaHotSpace[currentHotSpace].BeginRotationAnimation(pointer);
                    stop = new TimeSpan(DateTime.Now.Ticks);
                    //Console.WriteLine("Despues de iniciar animacion de rotacion del HotSpace numero " + nextHotSpace + " :" + PanoEngine.stop.Subtract(PanoEngine.start).TotalMilliseconds);
                    break;
                }
                
            }
        }

        #region Paseo Virtual Creation Methods

        private void procesarHotSpace(XmlReader reader)
        {
            reader.MoveToNextAttribute();
            int thisHotSpaceIndex = addHotSpace(reader.Value);

            //Leer norte, sur, este y oeste

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "HotSpace")
                {
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "North":
                                procesarMuro(reader, HotSpace.WALL.NORTH, thisHotSpaceIndex);
                                break;
                            case "West":
                                procesarMuro(reader, HotSpace.WALL.WEST, thisHotSpaceIndex);
                                break;
                            case "South":
                                procesarMuro(reader, HotSpace.WALL.SOUTH, thisHotSpaceIndex);
                                break;
                            case "East":
                                procesarMuro(reader, HotSpace.WALL.EAST, thisHotSpaceIndex);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo auxiliar. Se encarga de procesar un muro del HotSpace.
        /// </summary>
        /// <param name="reader">Reader XML con el archivo asociado.</param>
        /// <param name="paseo">PaseoVirtual que se esta modificando.</param>
        /// <param name="wall">Identificador del wall que se esta editando.</param>
        /// <param name="hotSpaceIndex">Indice dentro de paseo que indica el HotSpace que se esta editando.</param>
        /// <returns>El paseo virtual editado</returns>
        private void procesarMuro(XmlReader reader, HotSpace.WALL wall, int hotSpaceIndex)
        {
            reader.MoveToNextAttribute();
            if (reader.Name == "img")
            {
                addTextureToHotSpace(hotSpaceIndex, @reader.Value, wall);
            }


            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == mapWallString[wall])
                {
                    break;
                }


                if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "GoTo":
                            int x = 0, y = 0;
                            String where = null;
                            for (int j = 0; j < 3; ++j)
                            {
                                reader.MoveToNextAttribute();

                                switch (reader.Name)
                                {
                                    case "coordX":
                                        x = Convert.ToInt32(reader.Value);
                                        break;
                                    case "coordY":
                                        y = Convert.ToInt32(reader.Value);
                                        break;
                                    case "where":
                                        where = reader.Value;
                                        break;
                                    default:
                                        break;
                                }

                            }
                            addGoToToSpace(hotSpaceIndex, x, y, wall, where);

                            reader.Read();
                            break;
                        case "HotSpot":
                            reader.Read();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private int addHotSpace(string name){
            listaHotSpace.Add(new HotSpace(name));
            
            return listaHotSpace.Count - 1;
        }

        private void addTextureToHotSpace(int index, String textureP, HotSpace.WALL wall) {
            listaHotSpace[index].SetTextureToFace(wall, textureP);
        }

        private void addHotSpotToSpace(int index, int coordX, int coordY, HotSpace.WALL wall, HotSpot.ClickEventHandler handler) {
            listaHotSpace[index].addHotSpot(coordX, coordY, wall, 1.0f, handler);
        }

        private void addGoToToSpace(int index, int coordX, int coordY, HotSpace.WALL wall, string destinyName) {
            listaHotSpace[index].addGoTo(coordX, coordY, wall, 0.2f, destinyName, new GoTo.ClickGoToHandler(GoToClickHandler));
        }
        #endregion
    }
}
