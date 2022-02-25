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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonDown))]
        private static void GetButtonDownPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                switch (button)
                {
                    case Button.Select:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.PrimaryHand.InputSource].stateDown;
                        break;

                    case Button.AltSelect:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.SecondaryHand.InputSource].stateDown;
                        break;
                    
                    // @Refactor: none of the dpad buttons work at all
                    case Button.LookUp:
                    case Button.DirUp:
                        __result = SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.LeftHand].stateDown;
                        break;

                    case Button.LookDown:
                    case Button.DirDown:
                        __result = SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.LeftHand].stateDown;
                        break;

                    case Button.LookLeft:
                    case Button.DirLeft:
                        __result = SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.LeftHand].stateDown;
                        break;

                    case Button.LookRight:
                    case Button.DirRight:
                        __result = SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.LeftHand].stateDown;
                        break;

                    case Button.Menu:
                        __result = SteamVR_Actions._default.BClick[SteamVR_Input_Sources.LeftHand].stateDown;
                        break;

                    default:
                        break;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonUp))]
        private static void GetButtonUpPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                switch (button)
                {
                    case Button.Select:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.PrimaryHand.InputSource].stateUp;
                        break;

                    case Button.AltSelect:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.SecondaryHand.InputSource].stateUp;
                        break;

                    case Button.LookUp:
                    case Button.DirUp:
                        __result = SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.LeftHand].stateUp;
                        break;

                    case Button.LookDown:
                    case Button.DirDown:
                        __result = SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.LeftHand].stateUp;
                        break;

                    case Button.LookLeft:
                    case Button.DirLeft:
                        __result = SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.LeftHand].stateUp;
                        break;

                    case Button.LookRight:
                    case Button.DirRight:
                        __result = SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.LeftHand].stateUp;
                        break;

                    case Button.Menu:
                        __result = SteamVR_Actions._default.BClick[SteamVR_Input_Sources.RightHand].stateUp;
                        break;

                    default:
                        break;
                }
            }
        }

#pragma warning restore Harmony003
    }
}
