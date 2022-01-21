﻿using MonoMod.Cil;
using HarmonyLib;
using DiskCardGame;
using System.Reflection;

#pragma warning disable Publicizer001

namespace InscryptionVR.Modules
{
    internal static class HarmonyPatches
    {
        public static Harmony HarmonyIntance { get; private set; }

        public static void Init()
        {
            HarmonyIntance = new Harmony(PluginInfo.GUID);

            VRPlugin.Logger?.LogInfo("Harmony Patching...");
            HarmonyIntance.PatchAll(typeof(HarmonyPatches));
            VRPlugin.Logger?.LogInfo("Patching Complete");
        }


        [HarmonyILManipulator]
        [HarmonyPatch(typeof(ReferenceResolutionSetter), "CalculateReferenceHeight")]
        private static void FixPixelationIL(ILContext il)
        {
            var c = new ILCursor(il); // steps through IL code of patched method

            c.GotoNext(x => x.MatchLdcI4(550));
            c.Next.Operand = 1100;

            c.GotoNext(x => x.MatchLdcI4(550));
            c.Next.Operand = 1100;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PixelCamera), "LateUpdate")]
        private static void PixelCameraGBCTogglePatch(PixelCamera __instance)
        {
            if (Configs.EnableGBCToggle.Value) return;

            __instance.gbcMode = false;

            //typeof(PixelCamera).GetField("gbcMode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OpponentAnimationController), "SetExplorationModeLookTarget")]
        private static bool ExplorationLookTargetPatch(OpponentAnimationController __instance)
        {
            __instance.SetLookTarget(ViewManager.Instance.CameraParent.Find("Pixel Camera"), UnityEngine.Vector3.zero);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OpponentAnimationController), "ClearLookTarget")]
        private static bool ClearLookTargetPatch(OpponentAnimationController __instance)
        {
            //var type = typeof(OpponentAnimationController);
            //var bFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            __instance.lookTarget = ViewManager.Instance.pixelCamera.transform;
            __instance.lookOffset = UnityEngine.Vector3.zero;

            //type.GetField("lookTarget", bFlags).SetValue(__instance, ViewManager.Instance.CameraParent.Find("Pixel Camera"));
            //type.GetField("lookOffset", bFlags).SetValue(__instance, UnityEngine.Vector3.zero);

            return false;
        }
    }
}
