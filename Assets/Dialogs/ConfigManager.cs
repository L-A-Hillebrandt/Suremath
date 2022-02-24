using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Dialogs
{
    /// <summary>
    /// Class responsible for reading and writing config files
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// The standard file name for config files
        /// </summary>
        private const string ConfigFileName = "suremath-config.xml";
        /// <summary>
        /// The file path where exercise files are supposed to be stored on Windows
        /// </summary>
        private static readonly string WinExercisePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop";
        /// <summary>
        /// The default URL where the list of exercises can be accessed on the web
        /// </summary>
        private const string DefaultExerciseListUrl = @"http://127.0.0.1:3000/list-app";
        /// <summary>
        /// The default URL where a single exercise can be accessed on the web. Append a slash and an ID to fetch one.
        /// </summary>
        private const string DefaultExerciseUrl = @"http://127.0.0.1:3000/list";

        /// <summary>
        /// Creates a new config file with default values and writes it to the current working directory.
        /// </summary>
        private static void CreateNewConfig()
        {
            var writerSettings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            var writer = XmlWriter.Create(Application.dataPath + "/" + ConfigFileName, writerSettings);

            writer.WriteStartDocument();

            writer.WriteStartElement("suremath-config");
            writer.WriteAttributeString("version", "1.0");

            writer.WriteStartElement("exercise-paths");

            writer.WriteStartElement("local");
            writer.WriteString(WinExercisePath);
            writer.WriteEndElement();

            writer.WriteStartElement("online");

            writer.WriteStartElement("list");
            writer.WriteString(DefaultExerciseListUrl);
            writer.WriteEndElement();

            writer.WriteStartElement("exercise");
            writer.WriteString(DefaultExerciseUrl);

            writer.WriteEndDocument();
            writer.Close();

            Debug.Log("Config file written to " + Application.dataPath);
        }


        /// <summary>
        /// Reads in values from the config file and returns a Config object
        /// </summary>
        /// <returns>A Config objects containing the values read from the config file</returns>
        public static Config ReadConfig()
        {
            var localPath = Application.dataPath;

            //If we can't find a config file, make a new one with default values
            if (!File.Exists(localPath + "/" + ConfigFileName))
            {
                CreateNewConfig();
            }

            var configFile = XDocument.Load(Application.dataPath + "/" + ConfigFileName);

            var localExercisePath = from p in configFile.Descendants("local")
                select p.Value;

            var exercisePath = localExercisePath.First();

            var listUrl = from p in configFile.Descendants("list")
                select p.Value;

            var exerciseListUrl = listUrl.First();

            var exerciseUrlEnumerable = from p in configFile.Descendants("exercise")
                select p.Value;

            var exerciseUrl = exerciseUrlEnumerable.First();

            return new Config(exercisePath, exerciseListUrl, exerciseUrl);
        }
    }
}
