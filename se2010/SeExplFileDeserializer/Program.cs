using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.SpecExplorer.ObjectModel;

namespace SeExplFileDeserializer
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToSeExpl = args[0];
            ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(pathToSeExpl);
            string pathToSeExplFile = Path.GetDirectoryName(pathToSeExpl);
            Guid guid = Guid.NewGuid();
            string pathToXmlFile = Path.Combine(pathToSeExplFile, guid.ToString() + ".xml");
            ExplorationResult explorationResult = explorationResultLoader.LoadExplorationResult();

            SharedEntitySet sharedEntitySet = explorationResult.SharedEntities;
            TransitionSystem transitionSystem = explorationResult.TransitionSystem;
            List<StateEntity> se = explorationResult.StateEntities.ToList<StateEntity>();
            FileStream fs = new FileStream(pathToXmlFile, FileMode.CreateNew);
            XmlSerializer xmls = new XmlSerializer(typeof(TransitionSystem));
            xmls.Serialize((Stream)fs, (object)explorationResult.TransitionSystem);
            xmls = new XmlSerializer(typeof(SharedEntitySet));
            xmls.Serialize((Stream)fs, (object)explorationResult.SharedEntities);
            xmls = new XmlSerializer(typeof(List<StateEntity>));
            xmls.Serialize((Stream)fs, (object)explorationResult.StateEntities.ToList<StateEntity>());   

            Console.WriteLine("original .sexpl file name: {0} ", pathToSeExpl);
            Console.WriteLine("new .xml file name: {0}", pathToXmlFile);


            fs.Close();

            Console.Read();

        }
    }
}
