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
            var handTransform = typeof(AlternateInputInteractable).IsAssignableFrom(searchType) 
                ? VRController.SecondaryHand.transform 
                : VRController.PrimaryHand.transform;

            var ray = new Ray(handTransform.position, handTransform.forward);
            
            InteractableBase interactable = default;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
            {
                interactable = (InteractableBase)hit.transform.GetComponent(searchType);
            }

            return interactable;
        }

#pragma warning disable Harmony003

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButton))]
        private static void GetButtonPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                __result = button.GetVRInput(a => a.state);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonDown))]
        private static void GetButtonDownPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                __result = button.GetVRInput(a => a.stateDown);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonUp))]
        private static void GetButtonUpPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                __result = button.GetVRInput(a => a.stateUp);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonRepeating))]
        private static void GetButtonRepeatingPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                __result = button.GetVRInput(a => a.state);
            }
        }

#pragma warning restore Harmony003
    }
}
