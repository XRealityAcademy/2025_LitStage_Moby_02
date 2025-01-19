using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Meta.XR.Util;
using TMPro;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Meta.XR.MRUtilityKit
{
    public class DetectWall : MonoBehaviour
    {
        private OVRCameraRig _cameraRig;
        private Vector3 instantiatePosition = new Vector3(0, 0, 0);
        private Quaternion instantiateRotation = Quaternion.identity;
        public GameObject _debugCube;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Instantiate(_debugCube,instantiatePosition, instantiateRotation);
            
        }

        // Update is called once per frame
        void Update()
        {
           // GetBestPoseFromRaycastDebugger();
            if(OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                GetBestPoseFromRaycastDebugger();
            }
            
            
        }





       public void GetBestPoseFromRaycastDebugger()
        {
            
               // Instantiate(_debugCube, new Vector3(1,1,1), instantiateRotation);
                var ray = GetControllerRay();
                Instantiate(_debugCube, ray.origin, Quaternion.identity);
                MRUKAnchor sceneAnchor = null;
                var positioningMethod = MRUK.PositioningMethod.DEFAULT;
                var bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(ray, Mathf.Infinity,
                new LabelFilter(), out sceneAnchor, positioningMethod);
                if (bestPose.HasValue && sceneAnchor && _debugCube)
                {
                    _debugCube.transform.position = bestPose.Value.position;
                    _debugCube.transform.rotation = bestPose.Value.rotation;
                    _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);                            
                }
            
        }

        private Ray GetControllerRay()
        {
            Vector3 rayOrigin;
            Vector3 rayDirection;
            if (OVRInput.activeControllerType == OVRInput.Controller.Touch
                || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
            {
                rayOrigin = _cameraRig.rightHandOnControllerAnchor.position;
                rayDirection = _cameraRig.rightHandOnControllerAnchor.forward;
            }
            else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
            {
                rayOrigin = _cameraRig.leftHandOnControllerAnchor.position;
                rayDirection = _cameraRig.leftHandOnControllerAnchor.forward;
            }
            else // hands
            {
                var rightHand = _cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
                // can be null if running in Editor with Meta Linq app and the headset is put off
                if (rightHand != null)
                {
                    rayOrigin = rightHand.PointerPose.position;
                    rayDirection = rightHand.PointerPose.forward;
                }
                else
                {
                    rayOrigin = _cameraRig.centerEyeAnchor.position;
                    rayDirection = _cameraRig.centerEyeAnchor.forward;
                }
            }
            return new Ray(rayOrigin, rayDirection);

        }
    }
}
