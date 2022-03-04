using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DiskCardGame;
using Valve.VR;

namespace InscryptionVR.Modules.Mono
{
    public class HandController : MonoBehaviour
    {
        public SteamVR_Input_Sources InputSource 
        { 
            get => handedness == Hand.Right ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand; 
        }
        public Transform RaycastOrigin => handModel;
        
        public Hand handedness;

        public Transform handModel; //  Assumes origin is in wrist!
        public Transform handModelTarget;
        public Vector3 positionOffset = new(0.025f, 0.05f, -0.15f);
        public Vector3 rotationOffset = new(45f, 0f, 0f);

        public Transform gripPoint;


        private void Start()
        {
            transform.Find("Pointer").SetParent(RaycastOrigin, false);
        }

        private void LateUpdate()
        {
            if (handModelTarget == null)
                handModelTarget = transform;

            //if (RaycastOrigin == null)
            //    RaycastOrigin = transform;

            var pOffset = Vector3.Scale(positionOffset, handedness == Hand.Left ? new Vector3(-1f,1f,1f) : Vector3.one);
            
            handModel.SetPositionAndRotation
            (
                handModelTarget.position + handModelTarget.TransformVector(pOffset), 
                handModelTarget.rotation * Quaternion.Euler(rotationOffset)
            );
        }
    }
}