using System;
using DiskCardGame;
using UnityEngine;

namespace InscryptionVR.Modules
{
    internal static partial class HarmonyPatches
    {
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
    }
}
