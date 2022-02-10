using System;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Valve.VR;
using System.Reflection;

namespace InscryptionVR.Modules
{
    internal static partial class HarmonyPatches
    {
        //  Doesn't need Harmony attributes,
        //  gets patched in the VREnabler patcher
        public static InteractableBase RaycastForInteractableReplacement(int layerMask, Type searchType)
        {
            var handTransform = VRController.PrimaryHand.transform;
            var ray = new Ray(handTransform.position, handTransform.forward);
            
            InteractableBase interactable = default;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                interactable = (InteractableBase)hit.transform.GetComponent(searchType);
            }

            return interactable;
        }

#pragma warning disable Harmony003

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputButtons), "GetButtonDown")]
        private static bool GetButtonDownPatch(ref bool __result, Button button)
        {
            if (button == Button.Select && VRController.VRRigExists) 
            {
                __result = SteamVR_Actions.default_TriggerClick[VRController.PrimaryHand.InputSource].stateDown;
                return false;
            }

            if(button == Button.AltSelect && VRController.VRRigExists)
            {
                __result = SteamVR_Actions.default_TriggerClick[VRController.SecondaryHand.InputSource].stateDown;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputButtons), "GetButtonUp")]
        private static bool GetButtonUpPatch(ref bool __result, Button button)
        {
            if (button == Button.Select && VRController.VRRigExists)
            {
                __result = SteamVR_Actions.default_TriggerClick[VRController.PrimaryHand.InputSource].stateUp;
                return false;
            }

            if (button == Button.AltSelect && VRController.VRRigExists)
            {
                __result = SteamVR_Actions.default_TriggerClick[VRController.SecondaryHand.InputSource].stateUp;
                return false;
            }

            return true;
        }

#pragma warning restore Harmony003
    }
}
