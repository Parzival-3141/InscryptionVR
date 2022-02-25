using MonoMod.Cil;
using HarmonyLib;
using DiskCardGame;

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
        [HarmonyPatch(typeof(OpponentAnimationController), nameof(OpponentAnimationController.SetExplorationModeLookTarget))]
        private static bool ExplorationLookTargetPatch(OpponentAnimationController __instance)
        {
            __instance.SetLookTarget(ViewManager.Instance.CameraParent.Find("Pixel Camera"), UnityEngine.Vector3.zero);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OpponentAnimationController), nameof(OpponentAnimationController.ClearLookTarget))]
        private static bool ClearLookTargetPatch(OpponentAnimationController __instance)
        {
            __instance.lookTarget = ViewManager.Instance.pixelCamera.transform;
            __instance.lookOffset = UnityEngine.Vector3.zero;

            return false;
        }
    }
}
