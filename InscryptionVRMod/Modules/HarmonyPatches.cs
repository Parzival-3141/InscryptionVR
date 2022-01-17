using MonoMod.Cil;
using HarmonyLib;


namespace InscryptionVR.Modules
{
    internal static class HarmonyPatches
    {
        public static Harmony HarmonyIntance { get; private set; }

        public static void Init()
        {
            HarmonyIntance = new Harmony(PluginInfo.PLUGIN_GUID);

            VRPlugin.Logger?.LogInfo("Harmony Patching...");

            HarmonyIntance.PatchAll(typeof(HarmonyPatches));
            
            VRPlugin.Logger?.LogInfo("Harmony Patching Complete");
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
    }
}
