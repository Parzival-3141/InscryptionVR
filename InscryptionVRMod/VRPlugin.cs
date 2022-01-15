using System.Collections;
using BepInEx;
using UnityEngine;

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
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");

            //  Init data

            //  Subscribe to onLoad (if necessary)
        }


        private IEnumerator InitSteamVR()
        {
            yield return new WaitForSeconds(1f);
            //steamInitRunning = true;
            //SteamVR.Initialize(false);

            //while (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            //{
            //    if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeFailure)
            //    {
            //        MelonLogger.Error("[SteamVR] Initialization failure! Disabling VR modules.");
            //        vrEnabled = false;
            //        yield break;
            //    }
            //    yield return null;
            //}

            //steamInitRunning = false;

        }
    }
}
