using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Lamb; // CotL Assembly

namespace RedHeartsFirst
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "kel.cotl.redheartsfirst";
        public const string PluginName = "Red Hearts First";
        public const string PluginVer = "1.0.0";

        internal static ManualLogSource myLogger;

        private void Awake()
        {
            myLogger = Logger; // Make log source

            Logger.LogInfo($"Loaded {PluginName} successfully!");

            Harmony harmony = new Harmony("kel.harmony.redheartsfirst");
            harmony.PatchAll();

            FileLog.Reset();
        }
    }
}