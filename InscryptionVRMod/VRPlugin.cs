using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DiskCardGame;
using BepInEx.Logging;

namespace InscryptionVR
{
    internal static class PluginInfo
    {
        public const string PLUGIN_GUID = "parzival.inscryption.vrmod";
        public const string PLUGIN_NAME = "InscryptionVRMod";
        public const string PLUGIN_VERSION = "0.0.1";
    }


    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Inscryption.exe")]
    public class VRPlugin : BaseUnityPlugin
    {
        internal static Scene CurrentScene { get; private set; }
        internal static new ManualLogSource Logger { get; private set; }


        private bool vrEnabled;

        private void Awake()
        {
            Logger = base.Logger;

            Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} loaded");

            if (new List<string>(Environment.GetCommandLineArgs()).Contains("OpenVR"))
            {
                Logger.LogInfo("VR patches enabled");
                vrEnabled = true;
            }
            else
            {
                Logger.LogWarning("Launch parameter \"-vrmode\" not set to OpenVR, not loading VR patches!");

                vrEnabled = false;
                return;
            }

            //  Init data
            Modules.HarmonyPatches.Init();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

       

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (!vrEnabled) return;

            CurrentScene = scene;

            switch (CurrentScene.name)
            {
                case "Start":
                    Transform cam = Camera.main.transform;
                    cam.position += new Vector3(0, 0, 6);

                    StartCoroutine(SkipStartMenu());

                    break;

                case "Part1_Cabin":
                case "Part1_Vision":
                case "Part1_Sanctum":
                case "Part1_Finale":
                case "Part3_Cabin":
                    StartCoroutine(UIFixes());
                    break;

                default:
                    Logger.LogInfo($"Scene \"{scene}\" has no loading event!");
                    break;
            }

            StartCoroutine(InitSteamVR());
        }


        private IEnumerator SkipStartMenu()
        {
            yield return new WaitForSeconds(6f);

            if (!Input.GetKey(KeyCode.LeftAlt))
                yield break;

            var continueCard = GameObject.Find("MenuCard_Continue")?.GetComponent<MenuCard>();

            if (continueCard != null)
                GameObject.Find("StartMenu")?.GetComponent<MenuController>().PlayMenuCardImmediate(continueCard);
        }

        private IEnumerator UIFixes()
        {
            yield return null;

            //  Move ScreenEffects to prevent z-fighting
            UIManager.Instance.transform.Find("PerspectiveUICamera/ScreenEffects").localPosition += new Vector3(0f, 0f, 0.02f);
            
            //  Move TextDisplayer to a better location
            UIManager.Instance.transform.Find("PerspectiveUICamera/TextDisplayer").localPosition = new Vector3(0f, -0.2f, 1.05f);

            //  Move PauseMenu cam closer
            var camParent = new GameObject("CameraParent").transform;
            camParent.parent = UIManager.Instance.transform.Find("PauseMenu/MenuParent");
            
            var pixelCam = UIManager.Instance.transform.Find("PauseMenu/MenuParent/PixelCamera");
            pixelCam.parent = camParent;
            
            camParent.localPosition = new Vector3(0f, -1f, 6f);

            //  Disable Pause Camera gbcMode
            var bFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(PixelCamera).GetField("gbcMode", bFlags).SetValue(pixelCam.GetComponent<PixelCamera>(), false);
        }

        private IEnumerator InitSteamVR()
        {
            yield return null;

            //yield return new WaitForSeconds(1f);
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
