using HarmonyLib;
using DiskCardGame;
using MonoMod.Cil;

namespace InscryptionVRMod.Modules
{
    public static class CameraFixes
    {
        [HarmonyILManipulator]
        [HarmonyPatch(typeof(PixelCamera), "LateUpdate")]
        private static void FixPixelationIL(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(x => x.MatchLdcR4(100f));
            c.Next.Operand = 10f;
        }
    }
}
