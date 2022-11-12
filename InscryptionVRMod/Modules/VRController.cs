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
        //  Might be old

        public const bool shouldRenderRig = false;
        public const bool useBehaviourSkeleton = false;
        public const float rigScale = 6f;

        public static bool VRRigExists => RigInstance != null && RigInstance.isActiveAndEnabled;
        public static VRRig RigInstance { get; private set; }
        public static HandController PrimaryHand => Configs.IsLeftHanded.Value ? RigInstance.handLeft : RigInstance.handRight;
        public static HandController SecondaryHand => Configs.IsLeftHanded.Value ? RigInstance.handRight : RigInstance.handLeft;

        public static void InitRig()
        {
            VRPlugin.Logger.LogInfo("Creating VRRig...");
            VRPlugin.Logger.LogMessage("Ignore the script reference warnings");
            RigInstance = GameObject.Instantiate(Bundles.VRRigPrefab).GetComponent<VRRig>();
        }

        public static void SetupVRRig(GameObject VRRigPrefab, HandController left, HandController right)
        {
            var pArea = VRRigPrefab.AddComponent<SteamVR_PlayArea>();
            pArea.size = SteamVR_PlayArea.Size.Calibrated;
            pArea.drawInGame = shouldRenderRig;

            var rig = VRRigPrefab.AddComponent<VRRig>();
            rig.trackingParent = VRRigPrefab.transform.Find("TrackingParent");
            rig.calibratedCenter = rig.trackingParent.Find("Calibrated Center");
            rig.handLeft = left;
            rig.handRight = right;

            rig.trackingParent.localScale = Vector3.one * rigScale;
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

            if (useBehaviourSkeleton)
            {
                //  Behaviour Skeleton
                var skele = handObj.gameObject.AddComponent<SteamVR_Behaviour_Skeleton>();
                skele.skeletonAction = isRight ? SteamVR_Actions.default_SkeletonHandRight : SteamVR_Actions.default_SkeletonHandLeft;
                skele.inputSource = handObj.InputSource;
                skele.skeletonRoot = handObj.transform.Find("vr_glove_skeleton/Root");
                skele.mirroring = isRight ? SteamVR_Behaviour_Skeleton.MirrorType.None : SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft;
                skele.fallbackCurlAction = SteamVR_Actions.default_GripPull;

                VRPlugin.Logger.LogInfo("skele available: " + skele.skeletonAvailable
                    + " | skele action: " + skele.skeletonAction.fullPath
                    + " | skele active: " + skele.isActive);

                handObj.handModelTarget = skele.skeletonRoot.Find("wrist_r");
            }
            else
            {
                //  Behaviour Pose
                var pose = handObj.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
                pose.poseAction = SteamVR_Actions._default.Pose;
                pose.inputSource = handObj.InputSource;

                handObj.handModelTarget = null;
            }

            //  Hand Model
            handObj.handModel = handObj.transform.Find("Hand Model " + subfix1);
            handObj.handModel.transform.Find("mesh" + subfix2).
                GetComponent<SkinnedMeshRenderer>().material.shader = Bundles.HandDitherShader;
            handObj.gripPoint = handObj.handModel.Find("Grip Point");

            //  Ring Model
            string ringPath = $"{(isRight ? "fakeRoot/" : "")}hand{subfix2}/index1{subfix2}/RingParent/Ring";
            Transform ring = handObj.handModel.transform.Find(ringPath);
            ring.GetComponent<MeshRenderer>().material = UnityEngine.Resources.Load<Material>("art/assets3d/cabin/ring/Ring");

            var flag = ring.gameObject.AddComponent<DiskCardGame.ActiveIfStoryFlag>();
            flag.activeIfConditionMet = false;
            flag.checkConditionEveryFrame = false;
            flag.targetObject = null;
            flag.updateWhenPaused = false;
            flag.storyFlag = DiskCardGame.StoryEvent.Part3Completed;

            //ring.gameObject.SetActive(false);

            //  Controller Render Model
            //if (shouldRenderRig)
            //    handObj.transform.Find("Controller Model").gameObject.AddComponent<SteamVR_RenderModel>().shader = Shader.Find("Standard");

            return handObj;
        }
    }

    public enum Hand
    {
        Left,
        Right,
    }
}
