using BepInEx;
using System;
using System.IO;

namespace RedHeartsFirst
{
    internal static class SaveFile
    {
        static string savePath = null;

        public delegate void SaveEvent();
        public static event SaveEvent SaveActions;

        public static string SavePath
        {
            get
            {
                return savePath ?? FindSave();
            }
        }

        public static HeartOrder SaveData
        {
            get { return LoadSave(); }
            set { SaveToFile(value); }
        }

        static void SaveToFile (HeartOrder data)
        {
            int index = (int)data;
            File.WriteAllText(SavePath, index.ToString());

            SaveActions?.Invoke();
        }

        static HeartOrder LoadSave()
        {
            string data = File.ReadAllText(SavePath).Trim();
            bool flag = Int32.TryParse(data, out int index);
            if (!flag)
            {
                Plugin.myLogger.LogWarning($"Couldn't read config data from {Path.GetFileName(SavePath)}.");
                Plugin.myLogger.LogWarning("Heart order has defaulted to: Black, Red, Blue.");
                SaveData = HeartOrder.BlackRedBlue;
                return HeartOrder.BlackRedBlue;
            }
            return (HeartOrder)index;
        }

        static string FindSave()
        {
            string configName = "HeartOrder.txt";

            string[] files = Directory.GetFiles(Paths.PluginPath, configName, SearchOption.AllDirectories);

            if(files.Length == 0)
            {
                Plugin.myLogger.LogWarning($"Couldn't find file \"{configName}\". Creating that file instead.");
                string path = Path.Combine(Paths.PluginPath, configName);
                File.Create(path).Dispose();
                savePath = path;
                return savePath;
            }
            else if (files.Length > 1)
            {
                Plugin.myLogger.LogWarning($"Unexpected behavior: More than one file named \"{configName}\".");
            }

            savePath = files[0];
            return savePath;
        }
    }
}
