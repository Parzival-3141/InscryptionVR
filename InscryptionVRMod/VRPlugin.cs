using BepInEx;

namespace InscryptionVR
{
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "parzival.inscryption.vrmod";
        public const string PLUGIN_NAME = "InscryptionVRMod";
        public const string PLUGIN_VERSION = "0.0.1";
    }


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Inscryption.exe")]
    public class VRPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
