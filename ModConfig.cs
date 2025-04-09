using BepInEx.Configuration;

namespace REPOLevelScaler
{
    internal static class ModConfig
    {
        internal static ConfigEntry<bool> DeveloperLogging = null!;
        internal static ConfigEntry<int> ShrinkOdds = null!;
        internal static ConfigEntry<int> GrowOdds = null!;
        internal static ConfigEntry<bool> ScaleEnemies = null!;
        internal static ConfigEntry<bool> ScaleItems = null!;
        internal static ConfigEntry<bool> ScaleValuables = null!;
        internal static ConfigEntry<bool> ScaleWeights = null!;

        internal static void Init()
        {
            DeveloperLogging = Plugin.instance.Config.Bind("Debug", "Developer Logging", false, new ConfigDescription("Enable this to see developer logging output"));
            ShrinkOdds = Plugin.instance.Config.Bind("General", "Shrink Odds", 50, new ConfigDescription("The odds the level will shrink", new AcceptableValueRange<int>(0, 100)));
            GrowOdds = Plugin.instance.Config.Bind("General", "Grow Odds", 50, new ConfigDescription("The odds the level will grow", new AcceptableValueRange<int>(0, 100)));
            ScaleEnemies = Plugin.instance.Config.Bind("General", "Scale Enemies", true, new ConfigDescription("Enable or Disable scaling of enemies to match the new level size"));
            ScaleItems = Plugin.instance.Config.Bind("General", "Scale Items", true, new ConfigDescription("Enable or Disable scaling of Items to match the new level size (ie. the cart)"));
            ScaleValuables = Plugin.instance.Config.Bind("General", "Scale Valuables", true, new ConfigDescription("Enable or Disable scaling of Valuables to match the new level size"));

        }
    }
}
