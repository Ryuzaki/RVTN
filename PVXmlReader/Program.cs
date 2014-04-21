using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace PVXmlReader
{
    
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream fileStream = File.OpenRead("PaseoVirtual1.xml")) 
            {
                XmlReaderSettings settings;
                settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                using (XmlReader reader = XmlReader.Create(fileStream,settings)) 
                {
                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            switch(reader.Name){
                                case "PaseoVirtual":
                                    ProcesarPaseoVirtual(reader);
                                    break;
                                case "HotSpace":
                                    ProcesarHotSpace(reader);
                                    break;
                                default:
                                    System.Diagnostics.Debug.WriteLine("Etiqueta no procesada: " + reader.Name);
                                    break;
                            }
                        }
                    }


                }
            }

        }

        public static void ProcesarPaseoVirtual(XmlReader reader)
        {
            reader.MoveToNextAttribute();
            System.Diagnostics.Debug.WriteLine("Inicio del paseo virtual: ");
            if (reader.Name == "name") {
                System.Diagnostics.Debug.WriteLine(reader.Value);
            }
        }

        private static void ProcesarHotSpace(XmlReader reader) 
        {
            reader.MoveToNextAttribute();
            System.Diagnostics.Debug.WriteLine("Nombre del HotSpace: " + reader.Value);

            while (reader.Read()) { 
                
            }
        
        }

    }
}
