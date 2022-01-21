using System;
using BepInEx.Configuration;

namespace InscryptionVR.Modules
{
    internal static class Configs
    {
        public static ConfigFile File { get; private set; }

        public static ConfigEntry<bool> EnableGBCToggle { get; private set; }

        public static void Init(ConfigFile configFile)
        {
            File = configFile;

            EnableGBCToggle = File.Bind("General", "EnableGBCToggle", false, "Allows PixelCameras to toggle GBC mode.\n(Does reduce visibility)");
        }
    }
}
