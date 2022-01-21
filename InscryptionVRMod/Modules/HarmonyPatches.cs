using MonoMod.Cil;
using HarmonyLib;
using DiskCardGame;
using System.Reflection;

#pragma warning disable Publicizer001

namespace InscryptionVR.Modules
{
    internal static partial class HarmonyPatches
    {
        public static Harmony HarmonyIntance { get; private set; }

        public static void Init()
        {
            HarmonyIntance = new Harmony(PluginInfo.GUID);

            VRPlugin.Logger?.LogInfo("Harmony Patching...");
            HarmonyIntance.PatchAll(typeof(HarmonyPatches));
            VRPlugin.Logger?.LogInfo("Patching Complete");
        }


        //  Opponent Patches

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
