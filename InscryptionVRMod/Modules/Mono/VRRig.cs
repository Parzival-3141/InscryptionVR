using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DiskCardGame;

namespace InscryptionVR.Modules.Mono
{
    public class VRRig : MonoBehaviour
    {
        public Transform trackingParent;
        public Transform calibratedCenter;
        public Transform pixelCamParent;
        
        public bool doCorrection = true;
        public bool doRotation = true;

        public HandController handLeft, handRight;

        private void Start()
        {
            pixelCamParent = ViewManager.Instance.CameraParent;
            ViewManager.Instance.pixelCamera.transform.SetParent(trackingParent, true);
            ViewManager.Instance.pixelCamera.nearClipPlane = 0.01f;
        }

        private void LateUpdate()
        {
            if (!doCorrection) return;

            var posDelta = pixelCamParent.position - calibratedCenter.position;
            transform.position += posDelta;

            if (doRotation)
            {
                var rotDelta = pixelCamParent.rotation * Quaternion.Inverse(calibratedCenter.rotation);
                rotDelta.ToAngleAxis(out var angle, out var axis);
                transform.RotateAround(calibratedCenter.position, axis, angle);
            }
        }
    }
}
