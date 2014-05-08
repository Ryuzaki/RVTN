using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// El objetivo de PanoEngine es tener una clase estatica donde se reunan los elementos comunes que todas aquellas clases del espacio de nombre PanoEngine tienen en comun.
/// Además, muestra una interfaz para crear PaseoVirtual a partir de un archivo XML.
/// </summary>
namespace PanoEngine
{
    public class PanoEngine
    {
        #region Fields
        //private static PanoEngine _instance;

        //public Game _associatedGame;
        //public GraphicsDevice _graphicsDevice;
        //public ContentManager _content;
        
        //public Camera _camera;
        public int _pointerX;
        public int _pointerY;
        public bool PEPointerPressed;
        public bool PEPointerReleased;
        public bool PEPaseoCreated;
        public PaseoVirtual _lastPaseoVirtual;
        public bool MovementEnabled = true;

        //public static TimeSpan stop;
        //public static TimeSpan start;

        private static Dictionary<HotSpace.WALL, String> mapWallString = new Dictionary<HotSpace.WALL, string>()
        {
            {HotSpace.WALL.EAST, "East"},
            {HotSpace.WALL.NORTH, "North"},
            {HotSpace.WALL.SOUTH, "South"},
            {HotSpace.WALL.WEST, "West"}
        };

        #endregion

        #region Properties
        /*public static PanoEngine Instance
        {
            get {
                if (_instance == null) {
                    _instance = new PanoEngine();
                }
                return _instance;
            }
        }*/

        public PaseoVirtual LastPaseoVirtual
        {
            get
            {
                return _lastPaseoVirtual;
            }
        }
        /*public GraphicsDevice GraphicsDevice
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
        }*/

        #endregion

        #region Creation Methods

        /// <summary>
        /// Inicializa PanoEngine, esta llamada debe ser la primera antes de usar cualquier funcionalidad de la libreria.
        /// </summary>
        /// <param name="game">Game de XNA al que estará asociado el engine.</param>
        /// <param name="gD">GraphicsDevice del juego.</param>
        /// <param name="oldContent">Contenedor del juego del cual se hará una copia para usar los contenidos de la libreria</param>
        public void Initialize(Game game) 
        {
            _associatedGame = game;
            _graphicsDevice = game.GraphicsDevice;
            _content = new ContentManager(game.Content.ServiceProvider, "Content");
            _camera = new Camera(Vector3.Zero, new Vector3(0, 0, -1), Vector3.Up);
            game.Components.Add(_camera);

            _pointerX = 0;
            _pointerY = 0;
            PEPointerPressed = false;
            PEPointerReleased = false;
            PEPaseoCreated = false;
        }

        /// <summary>
        /// Crea un paseo virtual a partir de un archivo XML.
        /// </summary>
        /// <param name="pvfile">Archivo XML con la configuracion del PaseoVirtual.</param>
        /// <returns>El paseo virtual editado</returns>
        public void CreatePaseoVirtual(string pvfile) 
        {
            System.Diagnostics.Debug.WriteLine("Leyendo paseo virtual...");
            _lastPaseoVirtual = new PaseoVirtual();
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
                    _lastPaseoVirtual.Name = reader.Value;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "HotSpace")
                        {
                            if (reader.IsStartElement())
                            {
                                //<HotSpace>
                                _lastPaseoVirtual = procesarHotSpace(reader, _lastPaseoVirtual);
                            }
                        }
                    }
                }
                _lastPaseoVirtual.LoadContentCurrentHotSpace();
            }
            System.Diagnostics.Debug.WriteLine("Create Paseo Finished v1");
        }

        private void CreatePaseoFinished() 
        {
            System.Diagnostics.Debug.WriteLine("Create Paseo Finished v2");
            PEPaseoCreated = true;
        }
        #endregion


        #region Helpers

        /// <summary>
        /// Metodo auxiliar. Se encarga de procesar un HotSpace.
        /// </summary>
        /// <param name="reader">Reader XML con el archivo asociado.</param>
        /// <param name="paseo">PaseoVirtual que se esta modificando.</param>
        /// <returns>El paseo virtual editado</returns>
        private static PaseoVirtual procesarHotSpace(XmlReader reader, PaseoVirtual paseo)
        {
            reader.MoveToNextAttribute();
            int thisHotSpaceIndex = paseo.addHotSpace(reader.Value);

            //Leer norte, sur, este y oeste

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "HotSpace")
                {
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element) {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "North":
                                paseo = procesarMuro(reader,paseo, HotSpace.WALL.NORTH, thisHotSpaceIndex);
                                break;
                            case "West":
                                paseo = procesarMuro(reader,paseo, HotSpace.WALL.WEST, thisHotSpaceIndex);
                                break;
                            case "South":
                                paseo = procesarMuro(reader,paseo, HotSpace.WALL.SOUTH, thisHotSpaceIndex);
                                break;
                            case "East":
                                paseo = procesarMuro(reader,paseo, HotSpace.WALL.EAST, thisHotSpaceIndex);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            


            return paseo;
        }

        /// <summary>
        /// Metodo auxiliar. Se encarga de procesar un muro del HotSpace.
        /// </summary>
        /// <param name="reader">Reader XML con el archivo asociado.</param>
        /// <param name="paseo">PaseoVirtual que se esta modificando.</param>
        /// <param name="wall">Identificador del wall que se esta editando.</param>
        /// <param name="hotSpaceIndex">Indice dentro de paseo que indica el HotSpace que se esta editando.</param>
        /// <returns>El paseo virtual editado</returns>
        private static PaseoVirtual procesarMuro(XmlReader reader, PaseoVirtual paseo, HotSpace.WALL wall, int hotSpaceIndex)
        {
            reader.MoveToNextAttribute();
            if (reader.Name == "img")
            {
                paseo.addTextureToHotSpace(hotSpaceIndex, @reader.Value, wall);
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
                            paseo.addGoToToSpace(hotSpaceIndex, x, y, wall, where);

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


            
            return paseo;
        }
        #endregion
    }
}

//Informacion del HotSpace
                            