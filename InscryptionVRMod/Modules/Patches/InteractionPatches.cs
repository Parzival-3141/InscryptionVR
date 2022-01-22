using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil;
using Mono.Cecil.Cil;
using HarmonyLib;
using DiskCardGame;
using UnityEngine;
using Valve.VR;

namespace InscryptionVR.Modules
{
    internal static partial class HarmonyPatches
    {
        //  Patch InteractionCursor to use tracked controllers instead (?)

        //private static void UpdateCurrentInteractable<T>(InteractableBase current, string[] excludeLayers) where T : InteractableBase
        //{
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(InteractionCursor), "RaycastForInteractable")]
        //private static bool RaycastForInteractablePatch<T>(ref T __result, int layerMask, Vector3 cursorPosition) where T : InteractableBase
        //{
        //    __result = default;

        //    var handTransform = VRController.Rig.handRight.transform;
        //    var ray = new Ray(handTransform.position, handTransform.forward);

        //    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
        //    {
        //        __result = hit.transform.GetComponent<T>();
        //    }

        //    return false;
        //}


        [HarmonyILManipulator]
        [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.RaycastForInteractable))]
        private static void RedirectToNew(ILContext il)
        {
            ILCursor c = new(il);

            GenericInstanceMethod method = new(il.Import(AccessTools.Method(typeof(HarmonyPatches), nameof(JankPrefix))));
            method.GenericArguments.Add(il.Method.GenericParameters[0]);

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.Emit(OpCodes.Call, method);
            c.Emit(OpCodes.Ret);
        }

        private static T JankPrefix<T>(InteractionCursor __instance, int layerMask, Vector3 cursorPosition) where T : InteractableBase
        {
            // yeah do whatever
            
            var handTransform = VRController.Rig.handRight.transform;
            var ray = new Ray(handTransform.position, handTransform.forward);
            
            T interactable = default;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                interactable = hit.transform.GetComponent<T>();
            }

            return interactable;
            //return default;
        }








        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(InputButtons), "GetButtonDown")]
        //private static bool GetButtonDownPatch(ref bool __result, Button button)
        //{
        //    if(button == Button.Select)
        //    {
        //        __result = SteamVR_Actions.default_TriggerClick[SteamVR_Input_Sources.RightHand].stateDown;
        //        return false;
        //    }

        //    return true;
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(InputButtons), "GetButtonUp")]
        //private static bool GetButtonUpPatch(ref bool __result, Button button)
        //{
        //    if (button == Button.Select)
        //    {
        //        __result = SteamVR_Actions.default_TriggerClick[SteamVR_Input_Sources.RightHand].stateUp;
        //        return false;
        //    }

        //    return true;
        //}
    }
}
