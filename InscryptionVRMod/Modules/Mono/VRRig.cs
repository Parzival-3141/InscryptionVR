using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DiskCardGame;
using VR = Valve.VR;

namespace InscryptionVR.Modules.Mono
{
    public class VRRig : MonoBehaviour
    {
        public Transform trackingParent;
        public Transform calibratedCenter;
        
        public bool doCorrection = true;
        public bool doRotation = true;

        public HandController handLeft, handRight;
        
        private Transform pixelCamParent;

        private void Start()
        {
            pixelCamParent = ViewManager.Instance.CameraParent;
            ViewManager.Instance.pixelCamera.transform.SetParentScaled(trackingParent, true);
            ViewManager.Instance.pixelCamera.nearClipPlane = 0.01f;

            FirstPersonController.Instance.ItemHolder.heldItemParent = VRController.PrimaryHand.gripPoint;

            //RecalibrateHeight();
        }

        private void LateUpdate()
        {
            if (!doCorrection) return;

            //  @Refactor: add button in menu?
            if (VR.SteamVR_Actions._default.AClick[VRController.PrimaryHand.InputSource].stateDown)
            {
                RecalibrateHeight();
            }

            var posDelta = pixelCamParent.position - calibratedCenter.position;
            transform.position += posDelta;

            if (doRotation)
            {
                var rotDelta = pixelCamParent.rotation * Quaternion.Inverse(calibratedCenter.rotation);
                rotDelta.ToAngleAxis(out var angle, out var axis);
                transform.RotateAround(calibratedCenter.position, axis, angle);
            }
        }

        public void RecalibrateHeight()
        {
            calibratedCenter.localPosition = Vector3.up * ViewManager.Instance.pixelCamera.transform.localPosition.y;
        }
    }
}
