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
        public SteamVR_Input_Sources source;

        private void Awake()
        {
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose.poseAction = SteamVR_Actions.default_Pose;
            pose.origin = VRController.Rig.transform;
            pose.inputSource = source;

            var model = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            model.SetParent(transform, false);
            model.localScale = Vector3.one * 0.1f;
            model.GetComponent<Collider>().enabled = false;
        }
    }
}