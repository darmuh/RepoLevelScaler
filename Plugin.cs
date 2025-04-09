using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

namespace REPOLevelScaler
{
    public static class PlayerSizesInfo
    {
        public const string PLUGIN_GUID = "com.github.darmuh.REPOLevelScaler";
        public const string PLUGIN_NAME = "REPOLevelScaler";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInPlugin(PlayerSizesInfo.PLUGIN_GUID, PlayerSizesInfo.PLUGIN_NAME, PlayerSizesInfo.PLUGIN_VERSION)]
    //[BepInDependency("REPOLib", "2.0.0")]
    //[BepInDependency("nickklmao.menulib", "2.2.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance = null!;
        internal static ManualLogSource Log = null!;
        internal static System.Random Rand = new();

        private void Awake()
        {
            instance = this;
            Log = Logger;
            Log.LogInfo($"{PlayerSizesInfo.PLUGIN_NAME} is loading with version {PlayerSizesInfo.PLUGIN_VERSION}!");

            ModConfig.Init();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Log.LogInfo($"{PlayerSizesInfo.PLUGIN_NAME} load complete!");
        }

        internal static void Spam(string message)
        {
            if (ModConfig.DeveloperLogging.Value)
                Log.LogDebug(message);
        }

        internal static void Error(string message)
        {
            Log.LogError(message);
        }
    }
}
