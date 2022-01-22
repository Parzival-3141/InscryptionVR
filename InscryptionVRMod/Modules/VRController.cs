using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace InscryptionVR.Modules
{
    internal static class VRController
    {
        public static Mono.VRRig Rig { get; private set; }

        public static void Init()
        {
            CreateRig();
        }

        private static void CreateRig()
        {
            VRPlugin.Logger.LogInfo("Creating VRRig...");
            Rig = new GameObject("VRRig").AddComponent<Mono.VRRig>();

            Rig.calibratedCenter = new GameObject("CalibratedCenter").transform;
            Rig.calibratedCenter.SetParent(Rig.transform);
            Rig.calibratedCenter.localPosition = new Vector3(0f, 1.1f);

            Rig.handRight = CreateHand(Hand.Right);
            Rig.handLeft = CreateHand(Hand.Left);
        }

        public static Mono.HandController CreateHand(Hand hand)
        {
            var handObj = new GameObject("Hand" + hand.ToString()).AddComponent<Mono.HandController>();

            handObj.source = hand == Hand.Right ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
            handObj.transform.SetParent(Rig.transform);

            return handObj;
        }
    }

    public enum Hand
    {
        Left,
        Right,
    }
}
