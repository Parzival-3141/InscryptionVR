using MonoMod.Cil;
using HarmonyLib;
using DiskCardGame;

#pragma warning disable Publicizer001

namespace InscryptionVR.Modules
{
    internal static partial class HarmonyPatches
    {
        [HarmonyILManipulator]
        [HarmonyPatch(typeof(ReferenceResolutionSetter), nameof(ReferenceResolutionSetter.CalculateReferenceHeight))]
        private static void FixPixelationIL(ILContext il)
        {
            var c = new ILCursor(il); // steps through IL code of patched method

            c.GotoNext(x => x.MatchLdcI4(550));
            c.Next.Operand = 1100;

            c.GotoNext(x => x.MatchLdcI4(550));
            c.Next.Operand = 1100;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PixelCamera), nameof(PixelCamera.LateUpdate))]
        private static void PixelCameraGBCTogglePatch(PixelCamera __instance)
        {
            if (Configs.EnableGBCToggle.Value) return;

            __instance.gbcMode = false;
        }
    }
}
