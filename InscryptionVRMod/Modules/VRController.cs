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

        public const bool shouldRenderRig = false;
        public static bool VRRigExists => RigInstance != null && RigInstance.isActiveAndEnabled;
        public static VRRig RigInstance { get; private set; }
        public static HandController PrimaryHand => Configs.IsLeftHanded.Value ? RigInstance.handLeft : RigInstance.handRight;
        public static HandController SecondaryHand => Configs.IsLeftHanded.Value ? RigInstance.handRight : RigInstance.handLeft;

        public static void Init()
        {
            VRPlugin.Logger.LogInfo("Creating VRRig...");
            RigInstance = GameObject.Instantiate(Resources.VRRigPrefab).GetComponent<VRRig>();
        }

        public static void SetupVRRig(GameObject VRRigPrefab, HandController left, HandController right)
        {
            var pArea = VRRigPrefab.AddComponent<SteamVR_PlayArea>();
            pArea.size = SteamVR_PlayArea.Size.Calibrated;
            pArea.drawInGame = shouldRenderRig;

            var rig = VRRigPrefab.AddComponent<VRRig>();
            rig.calibratedCenter = VRRigPrefab.transform.Find("Calibrated Center");
            rig.handLeft = left;
            rig.handRight = right;
            
        }

        public static HandController SetupHand(Hand hand, GameObject VRRigPrefab)
        {
            bool isRight = hand == Hand.Right;
            string subfix1 = hand.ToString();
            string subfix2 = isRight ? "_r" : "_l";

            //  HandController
            var handObj = VRRigPrefab.transform.Find("TrackingParent/Controller " + subfix1)
                .gameObject.AddComponent<Mono.HandController>();
            handObj.handedness = hand;

            //  Behaviour Skeleton
            var skele = handObj.gameObject.AddComponent<SteamVR_Behaviour_Skeleton>();
            skele.skeletonAction = isRight ? SteamVR_Actions.default_SkeletonHandRight : SteamVR_Actions.default_SkeletonHandLeft;
            skele.inputSource = handObj.InputSource;
            skele.skeletonRoot = handObj.skeletonWrist.parent;
            skele.mirroring = isRight ? SteamVR_Behaviour_Skeleton.MirrorType.None : SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft;
            skele.fallbackCurlAction = SteamVR_Actions.default_GripPull;

            //  Model Positioning
            handObj.handModel = handObj.transform.Find("Hand Model " + subfix1);
            handObj.skeletonWrist = handObj.transform.Find("vr_glove_skeleton/Root/wrist_r");

            handObj.handModel.transform.Find("mesh" + subfix2).
                GetComponent<SkinnedMeshRenderer>().material.shader = Resources.HandDitherShader;

            //  Controller Render Model
            if (shouldRenderRig)
                handObj.transform.Find("Controller Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = Shader.Find("Standard");

            return handObj;
        }

        private static void CreateRig()
        {
            VRPlugin.Logger.LogInfo("Creating VRRig...");
            RigInstance = new GameObject("VRRig").AddComponent<VRRig>();

            RigInstance.calibratedCenter = new GameObject("CalibratedCenter").transform;
            RigInstance.calibratedCenter.SetParent(RigInstance.transform);
            RigInstance.calibratedCenter.localPosition = new Vector3(0f, 1.1f);

            RigInstance.handRight = CreateHand(Hand.Right);
            RigInstance.handLeft = CreateHand(Hand.Left);
        }

        private static Mono.HandController CreateHand(Hand hand)
        {
            var handObj = new GameObject("Hand" + hand.ToString()).AddComponent<HandController>();
            handObj.handedness = hand;
            handObj.transform.SetParent(RigInstance.transform);
            
            var pose = handObj.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            pose.poseAction = SteamVR_Actions.default_Pose;
            pose.origin = RigInstance.transform;
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
