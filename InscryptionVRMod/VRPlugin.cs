﻿using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DiskCardGame;
using BepInEx.Logging;
using InscryptionVR.Modules;
using Valve.VR;

namespace InscryptionVR
{
    internal static class PluginInfo
    {
        public const string GUID = "parzival.inscryption.vrmod";
        public const string NAME = "InscryptionVRMod";
        public const string VERSION = "0.0.1";
        public const string DESCRIPTION = "A VR mod for Inscryption";
    }


    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    [BepInProcess("Inscryption.exe")]
    public class VRPlugin : BaseUnityPlugin
    {
        internal static Scene CurrentScene { get; private set; }
        internal static new ManualLogSource Logger { get; private set; }


        private bool vrEnabled;
        private bool steamInitRunning;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"{PluginInfo.NAME} loaded");

            
            Modules.Configs.Init(Config);


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

            StartCoroutine(InitSteamVR());


            switch (CurrentScene.name)
            {
                case "Start":
                    StartCoroutine(StartMenuFixes());
                    StartCoroutine(SkipStartMenu());
                    break;

                case "Part1_Cabin":
                case "Part1_Vision":
                case "Part1_Sanctum":
                case "Part1_Finale":
                case "Part3_Cabin":
                    StartCoroutine(UIFixes());
                    VRController.Init();
                    break;

                default:
                    Logger.LogInfo($"Scene \"{scene}\" has no loading event!");
                    break;
            }

        }

        private IEnumerator InitSteamVR()
        {
            yield return new WaitForSeconds(1f);
            steamInitRunning = true;
            SteamVR.Initialize(false);

            while (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            {
                if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeFailure)
                {
                    Logger.LogError("[SteamVR] Initialization failure! Disabling VR modules.");
                    vrEnabled = false;
                    yield break;
                }
                yield return null;
            }

            steamInitRunning = false;
        }

        private IEnumerator StartMenuFixes()
        {
            yield return null;

            ReparentInPlace("PixelCameraParent", GBC.CameraController.Instance.transform.Find("PixelCamera"))
                .localPosition = new Vector3(0f, -1f, 6f);
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
            var pixelCam = UIManager.Instance.transform.Find("PauseMenu/MenuParent/PixelCamera");
            var camParent = ReparentInPlace("CameraParent", pixelCam);
            
            camParent.localPosition = new Vector3(0f, -1f, 6f);
        }

        private Transform ReparentInPlace(string newParentName, Transform toReparent)
        {
            var newParent = new GameObject(newParentName).transform;

            if (toReparent.parent != null)
                newParent.SetParent(toReparent.parent, false);
            
            toReparent.SetParent(newParent, true);

            return newParent;
        }
    }
}
