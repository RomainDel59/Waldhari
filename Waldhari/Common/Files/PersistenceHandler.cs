// This class is under MIT License,
// copyrighted to https://github.com/lucasvinbr,
// and comes from https://github.com/lucasvinbr/GTA5GangMod.
// I adapt it to my use.

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Waldhari.Common.UI;

namespace Waldhari.Common.Files
{
    /// <summary>
    /// This script controls all the saving and loading procedures called by the other scripts from this mod.
    /// </summary>
    public static class PersistenceHandler
    {
        public static string ModName = "Waldhari";

        public static string GetDirectory()
        {
            var dir = Application.StartupPath + "/scripts/" + ModName + "/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        /// <summary>
        /// attempts to load data from a file. The path is expected to be GTAV/scripts/{Logger.ModName}/{fileName}.xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string fileName)
        {
            Logger.Debug($"Attempting file load: {fileName} of type: {typeof(T)}");
            
            var serializer = new XmlSerializer(typeof(T));
            var filePath = GetDirectory() + fileName + ".xml";
            
            if (File.Exists(filePath))
            {
                try
                {
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        var loadedData = (T)serializer.Deserialize(readStream);
                        readStream.Close();
                        Logger.Debug($"{fileName} loaded!");
                        return loadedData;
                    }
                }
                catch (Exception e)
                {
                    NotificationHelper.Show($"Loading file {fileName} failed! Exception: {e}");
                    Logger.Exception($"Loading file {fileName} failed! Exception: {e}");
                    //backup the bad file! It's very sad to lose saved data, even if it's corrupted somehow
                    var bkpFilePath = GetDirectory() + fileName + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xml";
                    File.Copy(filePath, bkpFilePath, true);
                    return default;
                }
            }

            Logger.Info($"File {fileName} doesn't exist; loading a default setup.");
            return default;
        }

        public static void SaveToFile<T>(T dataToSave, string fileName)
        {
            try
            {
                Logger.Debug($"Attempting file save: {fileName} of type: {typeof(T)}");
                var serializer = new XmlSerializer(typeof(T));

                var filePath = GetDirectory() + fileName + ".xml";

                using (var writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, dataToSave);
                    writer.Close();
                }

                Logger.Debug("saved file successfully: " + fileName);
            }
            catch (Exception e)
            {
                NotificationHelper.Show($"Saving file {fileName} failed! Exception: {e}");
                Logger.Exception($"Saving file {fileName} failed! Exception: {e}");
            }
        }
    }
}
