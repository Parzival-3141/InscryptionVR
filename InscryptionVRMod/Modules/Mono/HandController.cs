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
            //var model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            //model.SetParent(transform, false);
            //model.localScale = Vector3.one * 0.1f;
            //model.GetComponent<Collider>().enabled = false;
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