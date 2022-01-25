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
        public Hand handedness;
        public SteamVR_Input_Sources InputSource 
        { 
            get => handedness == Hand.Right ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand; 
        }

        private void Awake()
        {
            var model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            model.SetParent(transform, false);
            model.localScale = Vector3.one * 0.1f;
            model.GetComponent<Collider>().enabled = false;
        }
    }
}