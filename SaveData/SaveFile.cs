using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UIElements;

namespace RedHeartsFirst
{
    internal static class SaveFile
    {
        static string savePath = null;

        public static string SavePath
        {
            get
            {
                return savePath ?? FindSave();
            }
        }

        public static HeartState SaveData
        {
            get { return LoadSave(); }
            set { SaveToFile(value); }
        }

        static void SaveToFile (HeartState data)
        {
            int index = (int)data;
            File.WriteAllText(SavePath, index.ToString());
            FileLog.Log($"Written {index}");
        }

        static HeartState LoadSave()
        {
            string data = File.ReadAllText(SavePath).Trim();
            bool flag = Int32.TryParse(data, out int index);
            if (!flag)
            {
                Plugin.myLogger.LogError($"Couldn't read config data from {Path.GetFileName(SavePath)}.");
                Plugin.myLogger.LogWarning("Heart order has defaulted to: Black, Red, Blue.");
                SaveData = HeartState.BlackRedBlue;
                return HeartState.BlackRedBlue;
            }
            return (HeartState)index;
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
                FileLog.Log("Here we go.");
                savePath = path;
                FileLog.Log("Ah.");
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
