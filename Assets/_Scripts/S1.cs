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
        void Update()
        {
            
        }
    }





}
