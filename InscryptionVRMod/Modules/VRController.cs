﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace InscryptionVR.Modules
{
    internal static class VRController
    {
        public static Mono.VRRig rig;

        public static void Init()
        {
            VRPlugin.Logger.LogInfo("Creating VRRig...");
            rig = new GameObject("VRRig").AddComponent<Mono.VRRig>();

            rig.calibratedCenter = new GameObject("CalibratedCenter").transform;
            rig.calibratedCenter.SetParent(rig.transform);
            rig.calibratedCenter.localPosition = new Vector3(0f, 1.1f);


            var handR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var poseR = handR.AddComponent<SteamVR_Behaviour_Pose>();
            poseR.poseAction = SteamVR_Actions.default_Pose;
            poseR.inputSource = SteamVR_Input_Sources.RightHand;
            poseR.origin = rig.transform;
            handR.transform.SetParent(rig.transform);


            var handL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var poseL = handL.AddComponent<SteamVR_Behaviour_Pose>();
            poseL.poseAction = SteamVR_Actions.default_Pose;
            poseL.inputSource = SteamVR_Input_Sources.LeftHand;
            poseL.origin = rig.transform;
            handL.transform.SetParent(rig.transform);
        }
    }
}
