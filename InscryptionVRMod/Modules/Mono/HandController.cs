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
        
        public Hand handedness;

        public Transform handModel; //  Assumes origin is in wrist!
        public Transform handTarget;
        public Vector3 positionOffset = new(0f, 0f, -0.02f);
        public Vector3 rotationOffset = new(0f, 0f, -14f);

        private void Awake()
        {
            var pointer = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            pointer.SetParent(transform, false);
            pointer.localScale = new Vector3(0.1f, 0.1f, 5f);
            pointer.localPosition = new Vector3(0f, 0f, 2.5f);
            Destroy(pointer.GetComponent<Collider>());
        }

        private void LateUpdate()
        {
            if(handTarget!= null)
            {
                var pOffset = handTarget.TransformVector(positionOffset);

                handModel.SetPositionAndRotation(handTarget.position + pOffset,
                    Quaternion.Euler(handTarget.rotation.eulerAngles + rotationOffset));
            }
        }
    }
}