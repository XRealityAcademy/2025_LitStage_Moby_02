using Meta.XR.MRUtilityKit;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    // [Header("References")]
    // [Tooltip("Prefab of the Table object, which contains the tabletop and cube.")]
    // public GameObject tablePrefab;

    [Header("References")]
    [Tooltip("Reference to the manager script S1.")]
    public S1 managerScript;

    private GameObject spawnedTable;
    private GameObject cube;
    private Transform rayOrigin;
    public LayerMask tableLayerMask;

    [Header("Settings")]
    [Tooltip("Maximum distance for the raycast.")]
    public float maxRayDistance = 100f;

    void Update()
    {
        // Draw a debug ray to visualize the ray direction in the Scene view.
        if (rayOrigin != null)
        {
            Debug.DrawRay(rayOrigin.position, rayOrigin.forward * maxRayDistance, Color.green);
        }

        // Check if the right-hand controller trigger is pressed.
        if (spawnedTable != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance, tableLayerMask))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f) // Ensure the hit is on a mostly horizontal surface
                {
                    Debug.Log("Ray hit " + hit.collider.name + " at position: " + hit.point);
                    Vector3 newPosition = hit.point;
                    newPosition.y = cube.transform.position.y; // Maintain original cube height
                    cube.transform.position = newPosition;
                }
                else
                {
                    Debug.Log("Ray hit an invalid surface, cube will not move.");
                }
            }
            else
            {
                Debug.Log("Ray did not hit the tabletop.");
            }
        }

        // Check if the player presses the A button to spawn the table
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) && spawnedTable == null)
        {
            SpawnTable();
        }
    }

    void SpawnTable()
    {
        spawnedTable = managerScript.GetSpawnedObject();
        if (spawnedTable != null)
        {
            cube = spawnedTable.transform.Find("Ishmael").gameObject;
            rayOrigin = GameObject.Find("RightHandAnchor").transform;
        }
        else
        {
            Debug.LogWarning("No spawned table found in S1 script.");
        }
    }
    
}
