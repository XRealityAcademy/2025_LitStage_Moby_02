using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Meta.XR.Util;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Meta.XR.MRUtilityKit
{
    public class PlaceMesh : MonoBehaviour
    {
        public S1 s1;
        public GameObject xQuad;
        public Material xQuadMaterial;
        public GameObject confirmUI;
        public GameObject mobyDickObj;
        public GameObject YCubePrefab;
        public GameObject PCubePrefab;
        [Tooltip("When false, use the interaction system already present in the scene")]
        public bool SetupInteractions;
        [Tooltip("Text field for displaying room details")]
        public TextMeshProUGUI RoomDetails;
        [Tooltip("Gaze pointer for VR interactions")]
        public OVRGazePointer GazePointer;
        [Tooltip("Input module for handling VR input")]
        public OVRInputModule InputModule;
        [Tooltip("Raycaster for handling ray interactions")]
        public OVRRaycaster Raycaster;
        [Tooltip("Helper for ray interactions")]
        public OVRRayHelper RayHelper;
        [Tooltip("Visualize anchors")] 
        public bool ShowDebugAnchors;
        public TMP_Dropdown positioningMethodDropdown;
        private Mesh _debugCheckerMesh;
        private bool _previousShowDebugAnchors;
        private readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");
        private readonly int _dstBlend = Shader.PropertyToID("_DstBlend");
        private readonly int _zWrite = Shader.PropertyToID("_ZWrite");
        private readonly int _cull = Shader.PropertyToID("_Cull");
        private readonly int _color = Shader.PropertyToID("_Color");        
        private readonly List<GameObject> _debugAnchors = new();
        private MRUKAnchor _previousShownDebugAnchor;
        private GameObject placedCube;
        // public MRUKAnchor MRUKAnchor;
        private GameObject m_placedCube;
        private GameObject m_debugCube;
        private EffectMesh _globalMeshEffectMesh;
        private SpaceMapGPU _spaceMapGPU;
        private GameObject _debugAnchor;
        private GameObject _debugCube;
        private GameObject _debugSphere;
        private Action _debugAction;
        private GameObject _debugNormal;
        private OVRCameraRig _cameraRig;
        [Tooltip(" Text field for displaying logs")]
        public TextMeshProUGUI logs;

        private void Awake()
        {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            if (SetupInteractions)
            {
                SetupInteractionDependencies();
            }
        }


        // Start is called before the first frame update
        private void Start()
        {
            MRUK.Instance?.RegisterSceneLoadedCallback(OnSceneLoaded);

            _globalMeshEffectMesh = GetGlobalMeshEffectMesh();
            _spaceMapGPU = GetSpaceMapGPU();
           // Instantiate(decoObj, Vector3.zero, Quaternion.identity);


        }

        // Update is called once per frame
        private void Update()
        {
            _debugAction?.Invoke();

            // Toggle the anchors debug visuals
            if (ShowDebugAnchors != _previousShowDebugAnchors)
            {
                if (ShowDebugAnchors)
                {
                    foreach (var room in MRUK.Instance.Rooms)
                    {
                        foreach (var anchor in room.Anchors)
                        {
                            GameObject anchorVisual = GenerateDebugAnchor(anchor);
                            _debugAnchors.Add(anchorVisual);
                        }
                    }
                }
                else
                {
                    foreach (var anchorVisual in _debugAnchors)
                    {
                        Destroy(anchorVisual.gameObject);
                    }

                    _previousShowDebugAnchors = ShowDebugAnchors;
                }
            }

        }

        private GameObject CreateDebugPrefabSource(bool isPlane)
        {
            var prefabName = isPlane ? "PlanePrefab" : "VolumePrefab";
            var prefabObject = new GameObject(prefabName);

            var meshParent = new GameObject("MeshParent");
            meshParent.transform.SetParent(prefabObject.transform);
            meshParent.SetActive(false);

            var prefabMesh = isPlane
                ? GameObject.CreatePrimitive(PrimitiveType.Quad)
                : GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefabMesh.name = "Mesh";
            prefabMesh.transform.SetParent(meshParent.transform);
            if (isPlane)
            // Unity quad's normal doesn't align with transform's Z-forward
            {
                prefabMesh.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            // Anchor cubes don't have a center pivot
            {
                prefabMesh.transform.localPosition = new Vector3(0, 0, -0.5f);
            }

            SetMaterialProperties(prefabMesh.GetComponent<MeshRenderer>());
            DestroyImmediate(prefabMesh.GetComponent<Collider>());

            var prefabPivot = new GameObject("Pivot");
            prefabPivot.transform.SetParent(prefabObject.transform);

            CreateGridPattern(prefabPivot.transform, Vector3.zero, Quaternion.identity);
            if (!isPlane)
            {
                CreateGridPattern(prefabPivot.transform, new Vector3(0, 0, -1), Quaternion.Euler(180, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(0, -0.5f, -0.5f), Quaternion.Euler(90, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(0, 0.5f, -0.5f), Quaternion.Euler(-90, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(-0.5f, 0, -0.5f), Quaternion.Euler(0, -90, 90));
                CreateGridPattern(prefabPivot.transform, new Vector3(0.5f, 0, -0.5f), Quaternion.Euler(180, -90, 90));
            }

            return prefabObject;
        }
        private void CreateGridPattern(Transform parentTransform, Vector3 localOffset, Quaternion localRotation)
        {
            var newGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newGameObject.name = "Checker";
            newGameObject.transform.SetParent(parentTransform, false);
            newGameObject.transform.localPosition = localOffset;
            newGameObject.transform.localRotation = localRotation;
            DestroyImmediate(newGameObject.GetComponent<Collider>());

            // offset the debug grid the smallest amount to avoid z-fighting
            const float NORMAL_OFFSET = 0.001f;

            // the mesh is used on every prefab, but only needs to be created once
            var vertCounter = 0;
            if (_debugCheckerMesh == null)
            {
                _debugCheckerMesh = new Mesh();
                const int gridWidth = 10;
                var cellWidth = 1.0f / gridWidth;
                var xPos = -0.5f;
                var yPos = -0.5f;

                var totalTiles = gridWidth * gridWidth / 2;
                var totalVertices = totalTiles * 4;
                var totalIndices = totalTiles * 6;

                var MeshVertices = new Vector3[totalVertices];
                var MeshUVs = new Vector2[totalVertices];
                var MeshColors = new Color32[totalVertices];
                var MeshNormals = new Vector3[totalVertices];
                var MeshTangents = new Vector4[totalVertices];
                var MeshTriangles = new int[totalIndices];

                var indexCounter = 0;
                var quadCounter = 0;

                for (var x = 0; x < gridWidth; x++)
                {
                    var createQuad = x % 2 == 0;
                    for (var y = 0; y < gridWidth; y++)
                    {
                        if (createQuad)
                        {
                            for (var V = 0; V < 4; V++)
                            {
                                var localVertPos = new Vector3(xPos, yPos + y * cellWidth, NORMAL_OFFSET);
                                switch (V)
                                {
                                    case 1:
                                        localVertPos += new Vector3(0, cellWidth, 0);
                                        break;
                                    case 2:
                                        localVertPos += new Vector3(cellWidth, cellWidth, 0);
                                        break;
                                    case 3:
                                        localVertPos += new Vector3(cellWidth, 0, 0);
                                        break;
                                }

                                MeshVertices[vertCounter] = localVertPos;
                                MeshUVs[vertCounter] = Vector2.zero;
                                MeshColors[vertCounter] = Color.black;
                                MeshNormals[vertCounter] = Vector3.forward;
                                MeshTangents[vertCounter] = Vector3.right;

                                vertCounter++;
                            }

                            var baseCount = quadCounter * 4;
                            MeshTriangles[indexCounter++] = baseCount;
                            MeshTriangles[indexCounter++] = baseCount + 2;
                            MeshTriangles[indexCounter++] = baseCount + 1;
                            MeshTriangles[indexCounter++] = baseCount;
                            MeshTriangles[indexCounter++] = baseCount + 3;
                            MeshTriangles[indexCounter++] = baseCount + 2;

                            quadCounter++;
                        }

                        createQuad = !createQuad;
                    }

                    xPos += cellWidth;
                }

                _debugCheckerMesh.Clear();
                _debugCheckerMesh.name = "CheckerMesh";
                _debugCheckerMesh.vertices = MeshVertices;
                _debugCheckerMesh.uv = MeshUVs;
                _debugCheckerMesh.colors32 = MeshColors;
                _debugCheckerMesh.triangles = MeshTriangles;
                _debugCheckerMesh.normals = MeshNormals;
                _debugCheckerMesh.tangents = MeshTangents;
                _debugCheckerMesh.RecalculateNormals();
                _debugCheckerMesh.RecalculateTangents();
            }

            newGameObject.GetComponent<MeshFilter>().mesh = _debugCheckerMesh;

            var material = newGameObject.GetComponent<MeshRenderer>().material;
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt(_srcBlend, (int)BlendMode.SrcAlpha);
            material.SetInt(_dstBlend, (int)BlendMode.One);
            material.SetInt(_zWrite, 0);
            material.SetInt(_cull, 2); // "Back"
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
       private GameObject CloneObject(GameObject prefabObj, Transform refObject)
        {
            var newObj = Instantiate(prefabObj);
            newObj.name = "Debug_" + refObject.name;
            newObj.transform.position = refObject.position;
            newObj.transform.rotation = refObject.rotation;

            return newObj;
        }

        private void SetMaterialProperties(MeshRenderer refMesh)
        {
            refMesh.material.SetColor(_color, new Color(0.5f, 0.9f, 1.0f, 0.75f));
            refMesh.material.SetOverrideTag("RenderType", "Transparent");
            refMesh.material.SetInt(_srcBlend, (int)BlendMode.SrcAlpha);
            refMesh.material.SetInt(_dstBlend, (int)BlendMode.One);
            refMesh.material.SetInt(_zWrite, 0);
            refMesh.material.SetInt(_cull, 2); // "Back"
            refMesh.material.DisableKeyword("_ALPHATEST_ON");
            refMesh.material.EnableKeyword("_ALPHABLEND_ON");
            refMesh.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            refMesh.material.renderQueue = (int)RenderQueue.Transparent;
        }
        private GameObject GenerateDebugAnchor(MRUKAnchor anchor)
        {
            var debugPlanePrefab = CreateDebugPrefabSource(true);
            var debugVolumePrefab = CreateDebugPrefabSource(false);

            Vector3 anchorScale;
            if (anchor.VolumeBounds.HasValue)
            {
                // Volumes
                _debugAnchor = CloneObject(debugVolumePrefab, anchor.transform);
                anchorScale = anchor.VolumeBounds.Value.size;
            }
            else
            {
                // Quads
                _debugAnchor = CloneObject(debugPlanePrefab, anchor.transform);
                anchorScale = Vector3.zero;
                if (anchor.PlaneRect != null)
                {
                    var quadScale = anchor.PlaneRect.Value.size;
                    anchorScale = new Vector3(quadScale.x, quadScale.y, 1.0f);
                }
            }

            ScaleChildren(_debugAnchor.transform, anchorScale);
            _debugAnchor.transform.parent = null;
            _debugAnchor.SetActive(true);

            Destroy(debugPlanePrefab);
            Destroy(debugVolumePrefab);

            return _debugAnchor;
        }
        private SpaceMapGPU GetSpaceMapGPU()
        {
            var spaceMaps = FindObjectsByType<SpaceMapGPU>(FindObjectsSortMode.None);
            return spaceMaps.Length > 0 ? spaceMaps[0] : null;
        }
        private void ScaleChildren(Transform parent, Vector3 localScale)
        {
            foreach (Transform child in parent)
            {
                child.localScale = localScale;
            }
        }

        private void ShowRoomDetails()
        {
            var currentRoomName = MRUK.Instance?.GetCurrentRoom().name ?? "N/A";
            var numRooms = MRUK.Instance?.Rooms.Count ?? 0;
            RoomDetails.text = string.Format("\n[{0}]\nNumber of rooms: {1}\nCurrent room: {2}",
                nameof(ShowRoomDetailsDebugger), numRooms, currentRoomName);
        }
        public void ShowRoomDetailsDebugger(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction += ShowRoomDetails;
                }
                else
                {
                    _debugAction -= ShowRoomDetails;
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(ShowRoomDetailsDebugger),
                    e.Message,
                    e.StackTrace);
            }
        }

        private EffectMesh GetGlobalMeshEffectMesh()
        {
            var effectMeshes = FindObjectsByType<EffectMesh>(FindObjectsSortMode.None);
            foreach (var effectMesh in effectMeshes)
            {
                if ((effectMesh.Labels & MRUKAnchor.SceneLabels.GLOBAL_MESH) != 0)
                {
                    return effectMesh;
                }
            }

            return null;
        }
        private void OnSceneLoaded()
        {
            CreateDebugPrimitives();
        }
        private void SetupInteractionDependencies()
        {
            if (!_cameraRig)
            {
                return;
            }

            GazePointer.rayTransform = _cameraRig.centerEyeAnchor;
            InputModule.rayTransform = _cameraRig.rightControllerAnchor;
            Raycaster.pointer = _cameraRig.rightControllerAnchor.gameObject;
            if (_cameraRig.GetComponentsInChildren<OVRRayHelper>(false).Length > 0)
            {
                return;
            }

            var rightControllerHelper =
                _cameraRig.rightControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
            if (rightControllerHelper)
            {
                rightControllerHelper.RayHelper =
                    Instantiate(RayHelper, Vector3.zero, Quaternion.identity, rightControllerHelper.transform);
                rightControllerHelper.RayHelper.gameObject.SetActive(true);
            }

            var leftControllerHelper =
                _cameraRig.leftControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
            if (leftControllerHelper)
            {
                leftControllerHelper.RayHelper =
                    Instantiate(RayHelper, Vector3.zero, Quaternion.identity, leftControllerHelper.transform);
                leftControllerHelper.RayHelper.gameObject.SetActive(true);
            }

            var hands = _cameraRig.GetComponentsInChildren<OVRHand>();
            foreach (var hand in hands)
            {
                hand.RayHelper =
                    Instantiate(RayHelper, Vector3.zero, Quaternion.identity, _cameraRig.trackingSpace);
                hand.RayHelper.gameObject.SetActive(true);
            }
        }
        private void CreateDebugPrimitives()
        {
            //_debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject XQuad  = Instantiate( xQuad, Vector3.zero, Quaternion.identity);
            _debugCube = XQuad;
            _debugCube.name = "SceneDebugger_Cube";
           // _debugCube.GetComponent<Renderer>().material=xQuadMaterial;
            _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _debugCube.GetComponent<Collider>().enabled = false;
            _debugCube.SetActive(true);

            _debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debugSphere.name = "SceneDebugger_Sphere";
            _debugSphere.GetComponent<Renderer>().material.color = Color.green;
            _debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _debugSphere.GetComponent<Collider>().enabled = false;
            _debugSphere.SetActive(false);

            _debugNormal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _debugNormal.name = "SceneDebugger_Normal";
            _debugNormal.GetComponent<Renderer>().material.color = Color.green;
            _debugNormal.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f);
            _debugNormal.GetComponent<Collider>().enabled = false;
            _debugNormal.SetActive(false);
        }
        private void SetLogsText(string logsText, params object[] args)
        {
            if (logs)
            {
                logs.text = string.Format(logsText, args);
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

        private void ShowHitNormal(Vector3 position, Vector3 normal)
        {
            if (_debugNormal != null && position != Vector3.zero && normal != Vector3.zero)
            {
                _debugNormal.SetActive(true);
                _debugNormal.transform.rotation = Quaternion.FromToRotation(-Vector3.up, normal);
                _debugNormal.transform.position =
                    position + -_debugNormal.transform.up * _debugNormal.transform.localScale.y;
            }
            else
            {
                _debugNormal.SetActive(false);
            }
        }


        public void GetClosestSurfacePositionDebugger(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction = () =>
                    {
                        var origin = GetControllerRay().origin;
                        var surfacePosition = Vector3.zero;
                        var normal = Vector3.up;
                        MRUKAnchor closestAnchor = null;
                        MRUK.Instance?.GetCurrentRoom()
                            ?.TryGetClosestSurfacePosition(origin, out surfacePosition, out closestAnchor, out normal);
                        ShowHitNormal(surfacePosition, normal);

                        if (closestAnchor != null)
                        {
                            SetLogsText("\n[{0}]\nAnchor: {1}\nSurface Position: {2}\nDistance: {3}",
                                nameof(GetClosestSurfacePositionDebugger),
                                closestAnchor.name,
                                surfacePosition,
                                Vector3.Distance(origin, surfacePosition).ToString("0.##")
                            );
                        }
                    };
                }
                else
                {
                    _debugAction = null;
                }

                if (_debugNormal != null)
                {
                    _debugNormal.SetActive(isOn);
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(GetClosestSurfacePositionDebugger),
                    e.Message,
                    e.StackTrace);
            }
        }

        public void SpawnOnFlatSurface(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction = () =>
                    {
                        var origin = GetControllerRay().origin;
                        var surfacePosition = Vector3.zero;
                        var normal = Vector3.up;
                        MRUKAnchor closestAnchor = null;
                        var ray = GetControllerRay();
                        MRUKAnchor sceneAnchor = null;
                        var positioningMethod = MRUK.PositioningMethod.DEFAULT;
                        if (positioningMethodDropdown)
                        {
                            positioningMethod = (MRUK.PositioningMethod)positioningMethodDropdown.value;
                        }
                        // Get the current room
                        var currentRoom = MRUK.Instance?.GetCurrentRoom();
                        if (currentRoom == null)
                        {
                            SetLogsText("\n[{0}]\nCurrent room is null!", nameof(SpawnOnFlatSurface));
                            return;
                        }
                        // Find the largest horizontal and vertical surfaces
                        var horizontalSurfaces = new[] {
                            MRUKAnchor.SceneLabels.TABLE,
                            MRUKAnchor.SceneLabels.BED,
                            MRUKAnchor.SceneLabels.FLOOR
                        };
                        var verticalSurfaces = new[] {
                            MRUKAnchor.SceneLabels.WALL_FACE,
                            MRUKAnchor.SceneLabels.SCREEN,
                            MRUKAnchor.SceneLabels.STORAGE,
                            MRUKAnchor.SceneLabels.CEILING
                        };
                        var bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(ray, Mathf.Infinity,
                            new LabelFilter(), out sceneAnchor, positioningMethod);
                        var surfaceNormalDir = MRUK.Instance?.GetCurrentRoom()?.GetFacingDirection(sceneAnchor);

                        MRUK.Instance?.GetCurrentRoom()
                            ?.TryGetClosestSurfacePosition(origin, out surfacePosition, out closestAnchor, out normal);
                        ShowHitNormal(surfacePosition, normal);

                       // var bestPose = currentRoom.GetBestPoseFromRaycast(ray, Mathf.Infinity, new LabelFilter(), out sceneAnchor, positioningMethod);
                        if (bestPose.HasValue && sceneAnchor != null && _debugCube != null)
                        {
                            _debugCube.transform.position = bestPose.Value.position;
                            _debugCube.transform.rotation = bestPose.Value.rotation;
                            // Check if the surface is horizontal or vertical
                            bool isHorizontal = false, isVertical = false;
                          
                            //Debug.Log("sceneAnchor.Label:" + sceneAnchor.Label.ToString()); //sceneAnchor.Label
                            if (sceneAnchor.HasAnyLabel(MRUKAnchor.SceneLabels.TABLE) || sceneAnchor.HasAnyLabel(MRUKAnchor.SceneLabels.BED)) 
                            { 
                                Debug.Log("sceneAnchor.Label:" + sceneAnchor.Label.ToString()+ " normal :" + normal.ToString() ); //sceneAnchor.Label
                                if (Vector3.Dot(normal, Vector3.up) > 0.4f) // Check if normal is pointing upwards (close to horizontal) 
                                { 
                                    isHorizontal = true; 
                                    
                                    //Debug.Log("sceneAnchor.Label:" + sceneAnchor.Label.ToString()+ "  Dot surfaceNormal :" + Vector3.Dot((Vector3)surfaceNormalDir, Vector3.up).ToString() ); //sceneAnchor.Label
                                } 
                                else 
                                { 
                                    isVertical = true; 
                                } 
                            } 
                            else 
                            { // General checks for other horizontal and vertical surfaces
                            
                                foreach (var label in horizontalSurfaces) 
                                { 
                                    if (sceneAnchor.HasAnyLabel(label)) 
                                    { 
                                        isHorizontal = true; 
                                        break; 
                                    } 
                                } 
                                
                                foreach (var label in verticalSurfaces) 
                                { 
                                    if (sceneAnchor.HasAnyLabel(label)) 
                                    { 
                                        isVertical = true; 
                                        break; 
                                    } 
                                }          
                            }

                            if (isHorizontal)
                            {
                                // Tint xQuad to yellow
                                _debugCube.GetComponent<Renderer>().material.color = Color.yellow;
                                if (OVRInput.GetDown(OVRInput.RawButton.A) && _debugCube != null)
                                {
                                    //confirmed UI shows up
                                    confirmUI.SetActive(true);

                                    //If the UI button "Yes" is pressed
                                    ////place the cube on the surface
                                    s1.PlaceGameObject(_debugCube.transform); 

                                    //If the UI button "No" is pressed
                                    ////deactive confirmed UI

                                    //yellow mark being place around
                                    // Vector3 offsetWall = _debugCube.transform.up;
                                    // mobyDickObj.transform.position =_debugCube.transform.position;
                                    // Vector3 wallOffset = _debugNormal.transform.right;
                                    // Vector3 mobyDickObj_pos = _debugCube.transform.position;


                                    
                                    
                                }
                            }
                            else if (isVertical)
                            {
                                // Tint xQuad to red 
                                _debugCube.GetComponent<Renderer>().material.color = Color.red;
                            }
                            SetLogsText("\n[{0}]\nAnchor: {1}\nPose Position: {2}\nPose Rotation: {3}",
                                nameof(SpawnOnFlatSurface),
                                sceneAnchor.name,
                                bestPose.Value.position,
                                bestPose.Value.rotation
                            );
                        }
                    };
                }
                else
                {
                    _debugAction = null;
                }
                if (_debugCube != null)
                {
                    _debugCube.SetActive(isOn);
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(SpawnOnFlatSurface),
                    e.Message,
                    e.StackTrace);
            }
        }

        public void RayCastDebugger(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction = () =>
                    {
                        var ray = GetControllerRay();
                        var hit = new RaycastHit();
                        MRUKAnchor anchorHit = null;
                        MRUK.Instance?.GetCurrentRoom()?.Raycast(ray, Mathf.Infinity, out hit, out anchorHit);
                        ShowHitNormal(hit.point, hit.normal);
                        if (anchorHit != null)
                        {
                            SetLogsText("\n[{0}]\nAnchor: {1}\nHit point: {2}\nHit normal: {3}\n",
                                nameof(RayCastDebugger),
                                anchorHit.name,
                                hit.point,
                                hit.normal
                            );
                        }
                    };
                }
                else
                {
                    _debugAction = null;
                }

                if (_debugNormal != null)
                {
                    _debugNormal.SetActive(isOn);
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(RayCastDebugger),
                    e.Message,
                    e.StackTrace
                );
            }
        }
        public void IsPositionInRoomDebugger(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction = () =>
                    {
                        var ray = GetControllerRay();
                        if (_debugSphere != null)
                        {
                            var isInRoom = MRUK.Instance?.GetCurrentRoom()
                                ?.IsPositionInRoom(_debugSphere.transform.position);
                            _debugSphere.transform.position = ray.GetPoint(0.2f); // add some offset
                            _debugSphere.GetComponent<Renderer>().material.color =
                                isInRoom.HasValue && isInRoom.Value ? Color.green : Color.red;
                            SetLogsText("\n[{0}]\nPosition: {1}\nIs inside the Room: {2}\n",
                                nameof(IsPositionInRoomDebugger),
                                _debugSphere.transform.position,
                                isInRoom
                            );
                        }
                    };
                }

                if (_debugSphere != null)
                {
                    _debugSphere.SetActive(isOn);
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(IsPositionInRoomDebugger),
                    e.Message,
                    e.StackTrace);
            }
        }
        public void ShowDebugAnchorsDebugger(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    _debugAction = () =>
                    {
                        var ray = GetControllerRay();
                        var hit = new RaycastHit();
                        MRUKAnchor anchorHit = null;
                        MRUK.Instance?.GetCurrentRoom()?.Raycast(ray, Mathf.Infinity, out hit, out anchorHit);
                        if (_previousShownDebugAnchor != anchorHit && anchorHit != null)
                        {
                            Destroy(_debugAnchor);
                            _debugAnchor = GenerateDebugAnchor(anchorHit);
                            _previousShownDebugAnchor = anchorHit;
                        }

                        ShowHitNormal(hit.point, hit.normal);
                        SetLogsText("\n[{0}]\nHit point: {1}\nHit normal: {2}\n",
                            nameof(ShowDebugAnchorsDebugger),
                            hit.point,
                            hit.normal
                        );
                    };
                }
                else
                {
                    _debugAction = null;
                    Destroy(_debugAnchor);
                    _debugAnchor = null;
                }

                if (_debugNormal != null)
                {
                    _debugNormal.SetActive(isOn);
                }
            }
            catch (Exception e)
            {
                SetLogsText("\n[{0}]\n {1}\n{2}",
                    nameof(ShowDebugAnchorsDebugger),
                    e.Message,
                    e.StackTrace);
            }
        }
        
    }
}
