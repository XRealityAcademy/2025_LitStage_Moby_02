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
        public GameObject[] gameUI;
        public GameObject[] storyObj;
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
            Instantiate(storyObj[0], t0.position, t0.rotation);
            //check if there's any object placed on the table, if yes, then deactive the X mark & storyObj & UI
            if(GameObject.Find(storyObj[0].name)!=null)
            {    
                placeMesh.SpawnOnFlatSurface(!isOn);            
                gameUI[0].SetActive (false);
            }
       
        }


    }





}
