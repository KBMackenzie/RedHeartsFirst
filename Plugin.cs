using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Lamb; // CotL Assembly
using Socket.Newtonsoft.Json.Utilities.LinqBridge;

namespace RedHeartsFirst
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [BepInDependency("kel.cotl.weaponselector", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid  = "kel.cotl.redheartsfirst";
        public const string PluginName  = "Red Hearts First";
        public const string PluginVer   = "1.0.0";

        internal static ManualLogSource myLogger;

        public static bool hasWeaponSelector = false;

        private void Awake()
        {
            myLogger = Logger; // Make log source

            Logger.LogInfo($"Loaded {PluginName} successfully!");

            // FileLog.Reset();

            Harmony harmony = new Harmony("kel.harmony.redheartsfirst");
            harmony.PatchAll();

            SaveFile.SaveEvent HeartUpdate = () => HeartPatches.Hearts = SaveFile.SaveData;

            SaveFile.SaveActions += HeartUpdate;
            HeartUpdate();

            // Make sure the menu doesn't overlap with Weapon Selector's menu
            string weaponSelectorGUID = "kel.cotl.weaponselector";
            hasWeaponSelector = Chainloader.PluginInfos.Any(x => x.Value.Metadata.GUID.Equals(weaponSelectorGUID));
        }
    }
}