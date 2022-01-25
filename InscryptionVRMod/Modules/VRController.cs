using System;
using System.Collections.Generic;
using System.Text;
using InscryptionVR.Modules.Mono;
using UnityEngine;
using Valve.VR;

namespace InscryptionVR.Modules
{
    internal static class VRController
    {
        //  @Refactor:
        //  Bug with steamvr nullref-ing after leaving the 
        //  cabin, can't find a BehaviourPose object?

        public static bool VRRigExists => Rig != null && Rig.isActiveAndEnabled;
        public static VRRig Rig { get; private set; }
        public static HandController PrimaryHand => Configs.IsLeftHanded.Value ? Rig.handLeft : Rig.handRight;
        public static HandController SecondaryHand => Configs.IsLeftHanded.Value ? Rig.handRight : Rig.handLeft;

        public static void Init()
        {
            CreateRig();
        }

        //  @Refactor: build VRRig in Unity and port over with an AssetBundle
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

        private static Mono.HandController CreateHand(Hand hand)
        {
            var handObj = new GameObject("Hand" + hand.ToString()).AddComponent<Mono.HandController>();
            handObj.handedness = hand;
            handObj.transform.SetParent(Rig.transform);
            
            var pose = handObj.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose.poseAction = SteamVR_Actions.default_Pose;
            pose.origin = Rig.transform;
            pose.inputSource = handObj.InputSource;

            return handObj;
        }
    }

    public enum Hand
    {
        Left,
        Right,
    }
}
