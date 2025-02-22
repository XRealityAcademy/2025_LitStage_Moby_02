using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.AI; // Optional: if you’re using a NavMeshAgent for movement


public class CharacterRayPointMove : MonoBehaviour
{
    public S1 s1;
    [Header("Raycast Settings")]
    [Tooltip("Reference to the right controller's transform")]
    public Transform rightControllerTransform;
    [Tooltip("Layer mask for the tabletop (ensure your table's collider is on this layer)")]
    public LayerMask tableLayer;
    [Tooltip("Maximum distance for the ray")]
    public float maxDistance = 100f;
  
    [Header("Visual Debugging")]
    [Tooltip("Optional LineRenderer to visualize the ray")]
    public LineRenderer lineRenderer;
    public float moveSpeed = 2f;

        // Private references to the character.
    private Transform characterTransform;
    private GameObject character;
    //private NavMeshAgent characterAgent;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        
        // Get the spawned object from S1; if not yet instantiated, use the prefab reference.
        GameObject spawnedObj = s1.GetSpawnedObject();
        if (spawnedObj == null)
        {
            // Warning: if the spawned object isn’t available yet, this may only work for testing.
            spawnedObj = s1.storyObj;
            Debug.LogWarning("Spawned object not found. Using prefab reference from storyObj[0].");
        }
        // Find the child named "Ishmael" which is your tiny virtual character.
        characterTransform = spawnedObj.transform.Find("Ishmael");
        if (characterTransform == null)
        {
            Debug.LogError("Child named 'Ishmael' not found in the spawned object.");
            return;
        }
        character = characterTransform.gameObject;
        // characterAgent = character.GetComponent<NavMeshAgent>();
        // if (characterAgent == null)
        // {
        //     Debug.LogWarning("NavMeshAgent not found on the character. Movement will be instantaneous.");
        // }

        if (rightControllerTransform == null)
        {
            Debug.LogWarning("Right Controller Transform is not assigned in the inspector.");
        }
    }

    void Update()
    {
                // Attempt to initialize the character if it hasn't been assigned yet.
        if (character == null && s1 != null)
        {
            GameObject spawnedObj = s1.GetSpawnedObject();
            if (spawnedObj != null)
            {
                characterTransform = spawnedObj.transform.Find("Ishmael");
                // if (characterTransform != null)
                // {
                //     character = characterTransform.gameObject;
                //     characterAgent = character.GetComponent<NavMeshAgent>();
                //     if (characterAgent == null)
                //     {
                //         Debug.LogWarning("NavMeshAgent not found on the character. Movement will be instantaneous.");
                //     }
                //     else
                //     {
                //         Debug.Log("Character successfully found and initialized.");
                //     }
                // }
                // else
                // {
                //     Debug.LogWarning("Child named 'Ishmael' not found in the spawned object.");
                // }
            }
        }
        
        // Ensure we have a reference to the right controller transform.
        if (rightControllerTransform == null)
            return;

        // Get the ray origin and direction from the right controller.
        Vector3 origin = rightControllerTransform.position;
        Vector3 direction = rightControllerTransform.forward;

        // Optional: Visualize the ray using a LineRenderer.
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);
        }

        // Check for the trigger press using OVRInput (for the right controller).
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.RTouch)||
        OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, tableLayer))
            {

                Debug.Log("Table hit at: " + hit.point);
                targetPosition = hit.point;
                isMoving = true;
            }
        }
        
        // If a target position is set, move Ishmael towards it.
        if (isMoving && character != null)
        {
            float step = moveSpeed * Time.deltaTime;
            character.transform.position = Vector3.MoveTowards(character.transform.position, targetPosition, step);
            // Check if Ishmael has reached close to the target.
            if (Vector3.Distance(character.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
                // Debug.Log("Table hit at: " + hit.point);
                // // Move the character to the hit point.
                // if (characterAgent != null)
                // {
                //     characterAgent.SetDestination(hit.point);
                // }
                // else if (character != null)
                // {
                //     // For testing purposes, you could instantly move the character:
                //     character.transform.position = hit.point;
                // }
            
        }
    }
}

