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
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{

    public class S1 : MonoBehaviour
    {
        public PlaceMesh placeMesh;
        public GameObject confirmedUI;
        public GameObject detectPlaneUI;
        public GameObject[] gameUI;
        public GameObject[] storyObj;
        private GameObject instantiatedStoryObj;
        private bool isOn;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            isOn = true;
            placeMesh.SpawnOnFlatSurface (isOn);
        }
        void Start()
        {

        }

        // Update is called once per frame
        public void PlaceGameObject(Transform t0)
        {
            if (instantiatedStoryObj == null) // Ensure no duplicate instantiations
            {
                instantiatedStoryObj = Instantiate(storyObj[0], t0.position, t0.rotation);
                Debug.Log($"Placed {instantiatedStoryObj.name} at {t0.position}");
                // Check if the object exists, then deactivate relevant UI
                if (instantiatedStoryObj != null)
                {
                    placeMesh.SpawnOnFlatSurface(!isOn);
                    gameUI[0].SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("An object is already placed. Remove it before placing another.");
            }
        }

        public void YesConfirm()
        {
            
            ////place the cube on the surface
           // PlaceGameObject(placeMesh._debugCube.transform); 
            confirmedUI.SetActive(false);

        }

        // Handle the "No" confirmation logic
        public void NoConfirm()
        {
            if (instantiatedStoryObj != null)
            {
                Destroy(instantiatedStoryObj);
                instantiatedStoryObj = null;
                Debug.Log("Destroyed the placed story object.");
            }
            else
            {
                Debug.LogWarning("No object to destroy.");
            }
            confirmedUI.SetActive(false);
            detectPlaneUI.SetActive(true);
            gameUI[0].SetActive(true);
            placeMesh.SpawnOnFlatSurface(isOn);
        }
        // Remove the placed object and reset the UI
        public void RemoveGameObject(Transform t0)
        {
            if (instantiatedStoryObj != null)
            {
                Destroy(instantiatedStoryObj);
                instantiatedStoryObj = null;
                Debug.Log("Removed the placed story object.");
                detectPlaneUI.SetActive(true);
                gameUI[0].SetActive(true);
                placeMesh.SpawnOnFlatSurface(isOn);
            }
            else
            {
                Debug.LogWarning("No object found to remove.");
            }
        }
        


    }



}
