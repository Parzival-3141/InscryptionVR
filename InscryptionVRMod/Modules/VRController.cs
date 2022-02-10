using System;
using System.Collections.Generic;
using System.Text;
using InscryptionVR.Modules.Mono;
using UnityEngine;
using Valve.VR;

#pragma warning disable Publicizer001

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
            rig.trackingParent = VRRigPrefab.transform.Find("TrackingParent");
            rig.calibratedCenter = VRRigPrefab.transform.Find("Calibrated Center");
            rig.handLeft = left;
            rig.handRight = right;
            
        }

        public static HandController SetupHand(Hand hand, GameObject VRRigPrefab)
        {
            bool isRight = hand == Hand.Right;
            string subfix1 = $"({hand})";
            string subfix2 = isRight ? "_r" : "_l";

            //  HandController
            var handObj = VRRigPrefab.transform.Find("TrackingParent/Controller " + subfix1)
                .gameObject.AddComponent<HandController>();
            handObj.handedness = hand;


            //  Behaviour Skeleton
            var skele = handObj.gameObject.AddComponent<SteamVR_Behaviour_Skeleton>();
            skele.skeletonAction = isRight ? SteamVR_Actions.default_SkeletonHandRight : SteamVR_Actions.default_SkeletonHandLeft;
            skele.inputSource = handObj.InputSource;
            skele.skeletonRoot = handObj.transform.Find("vr_glove_skeleton/Root");
            skele.mirroring = isRight ? SteamVR_Behaviour_Skeleton.MirrorType.None : SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft;
            skele.fallbackCurlAction = SteamVR_Actions.default_GripPull;


            //  Behaviour Pose
            //var pose = handObj.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
            //pose.poseAction = SteamVR_Actions._default.Pose;
            //pose.inputSource = handObj.InputSource;

            //  Model stuff
            handObj.handModel = handObj.transform.Find("Hand Model " + subfix1);
            handObj.handTarget = skele.skeletonRoot.Find("wrist_r");
            //handObj.handTarget = null;

            handObj.handModel.transform.Find("mesh" + subfix2).
                GetComponent<SkinnedMeshRenderer>().material.shader = Resources.HandDitherShader;
            //handObj.handModel.transform.Find("Ring").GetComponent<MeshRenderer>().material = Material.
            
            var ring = handObj.handModel.transform.Find("Ring");

            var flag = ring.gameObject.AddComponent<DiskCardGame.ActiveIfStoryFlag>();
            flag.activeIfConditionMet = false;
            flag.checkConditionEveryFrame = false;
            flag.targetObject = null;
            flag.updateWhenPaused = false;
            flag.storyFlag = DiskCardGame.StoryEvent.Part3Completed;

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
