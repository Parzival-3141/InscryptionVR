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
            if (VRController.VRRigExists)
            {
                switch (button)
                {
                    case Button.Select:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.PrimaryHand.InputSource].stateDown;
                        return false;

                    case Button.AltSelect:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.SecondaryHand.InputSource].stateDown;
                        return false;
                    
                    // @Incomplete: none of the dpad buttons work at all
                    case Button.LookUp:
                    case Button.DirUp:
                        __result = SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.RightHand].stateDown;
                        VRPlugin.Logger.LogInfo("Up " + __result);
                        return false;

                    case Button.LookDown:
                    case Button.DirDown:
                        VRPlugin.Logger.LogInfo("Down " + __result);
                        __result = SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.RightHand].stateDown;
                        return false;

                    case Button.LookLeft:
                    case Button.DirLeft:
                        VRPlugin.Logger.LogInfo("Left " + __result);
                        __result = SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.RightHand].stateDown;
                        return false;

                    case Button.LookRight:
                    case Button.DirRight:
                        VRPlugin.Logger.LogInfo("Right " + __result);
                        __result = SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.RightHand].stateDown;
                        return false;

                    case Button.Menu:
                        __result = SteamVR_Actions._default.BClick[SteamVR_Input_Sources.RightHand].stateDown;
                        return false;

                    default:
                        break;
                }
            }

            //if (button == Button.Select && VRController.VRRigExists) 
            //{
            //    __result = SteamVR_Actions.default_TriggerClick[VRController.PrimaryHand.InputSource].stateDown;
            //    return false;
            //}

            //if(button == Button.AltSelect && VRController.VRRigExists)
            //{
            //    __result = SteamVR_Actions.default_TriggerClick[VRController.SecondaryHand.InputSource].stateDown;
            //    return false;
            //}

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputButtons), "GetButtonUp")]
        private static bool GetButtonUpPatch(ref bool __result, Button button)
        {
            if (VRController.VRRigExists)
            {
                switch (button)
                {
                    case Button.Select:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.PrimaryHand.InputSource].stateUp;
                        return false;

                    case Button.AltSelect:
                        __result = SteamVR_Actions._default.TriggerClick[VRController.SecondaryHand.InputSource].stateUp;
                        return false;

                    case Button.LookUp:
                    case Button.DirUp:
                        __result = SteamVR_Actions._default.DPadUp[SteamVR_Input_Sources.LeftHand].stateUp;
                        return false;

                    case Button.LookDown:
                    case Button.DirDown:
                        __result = SteamVR_Actions._default.DPadDown[SteamVR_Input_Sources.LeftHand].stateUp;
                        return false;

                    case Button.LookLeft:
                    case Button.DirLeft:
                        __result = SteamVR_Actions._default.DPadLeft[SteamVR_Input_Sources.LeftHand].stateUp;
                        return false;

                    case Button.LookRight:
                    case Button.DirRight:
                        __result = SteamVR_Actions._default.DPadRight[SteamVR_Input_Sources.LeftHand].stateUp;
                        return false;

                    case Button.Menu:
                        __result = SteamVR_Actions._default.BClick[SteamVR_Input_Sources.RightHand].stateUp;
                        return false;

                    default:
                        break;
                }
            }

            //if (button == Button.Select && VRController.VRRigExists)
            //{
            //    __result = SteamVR_Actions.default_TriggerClick[VRController.PrimaryHand.InputSource].stateUp;
            //    return false;
            //}

            //if (button == Button.AltSelect && VRController.VRRigExists)
            //{
            //    __result = SteamVR_Actions.default_TriggerClick[VRController.SecondaryHand.InputSource].stateUp;
            //    return false;
            //}

            return true;
        }

#pragma warning restore Harmony003
    }
}
