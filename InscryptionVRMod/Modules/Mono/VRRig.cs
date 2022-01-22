using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DiskCardGame;

namespace InscryptionVR.Modules.Mono
{
    public class VRRig : MonoBehaviour
    {
        public Transform calibratedCenter;
        public Transform pixelCamParent;
        
        public bool doCorrection = true;
        public bool doRotation = true;

        public HandController handLeft, handRight;

        private void Start()
        {
            Transform cam = GameObject.Find("/FirstPersonController/PixelCameraParent/PixelCamera")?.transform;
            VRPlugin.Logger.LogInfo("Cam Test " + cam);
            pixelCamParent = cam.parent;
            cam.SetParent(transform, true);
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
