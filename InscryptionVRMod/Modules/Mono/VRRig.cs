using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DiskCardGame;

namespace InscryptionVR.Modules.Mono
{
    public class VRRig : MonoBehaviour
    {
        public Transform calibratedHeight;
        public Transform pixelCamParent;
        
        public bool doCorrection = true;
        public bool doRotation = true;

        private void Start()
        {
            var cam = FirstPersonController.Instance.transform.Find("PixelCameraParent/PixelCamera");
            Debug.Log(cam);
            pixelCamParent = cam.parent;
            cam.SetParent(transform, true);
        }

        private void LateUpdate()
        {
            if (!doCorrection) return;

            var posDelta = pixelCamParent.position - calibratedHeight.position;
            transform.position += posDelta;

            if (doRotation)
            {
                var rotDelta = pixelCamParent.rotation * Quaternion.Inverse(calibratedHeight.rotation);
                rotDelta.ToAngleAxis(out var angle, out var axis);
                transform.RotateAround(calibratedHeight.position, axis, angle);
            }
        }
    }
}
