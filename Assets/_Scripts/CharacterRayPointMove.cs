using UnityEngine;
using UnityEngine.AI; // Optional: if you’re using a NavMeshAgent for movement


public class CharacterRayPointMove : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Reference to the right controller's transform")]
    public Transform rightControllerTransform;
    [Tooltip("Layer mask for the tabletop (ensure your table's collider is on this layer)")]
    public LayerMask tableLayer;
    [Tooltip("Maximum distance for the ray")]
    public float maxDistance = 100f;
    
    [Header("Character Settings")]
    [Tooltip("The virtual character that will move to the hit point")]
    public GameObject character;
    // Optional: if your character uses a NavMeshAgent for pathfinding
    private NavMeshAgent characterAgent;
    
    [Header("Visual Debugging")]
    [Tooltip("Optional LineRenderer to visualize the ray")]
    public LineRenderer lineRenderer;

    void Start()
    {
        if (character != null)
        {
            characterAgent = character.GetComponent<NavMeshAgent>();
        }
        // Optionally, you can warn if the right controller reference is not assigned.
        if (rightControllerTransform == null)
        {
            Debug.LogWarning("Right Controller Transform is not assigned in the inspector.");
        }
    }

    void Update()
    {
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
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, tableLayer))
            {
                Debug.Log("Table hit at: " + hit.point);
                // Move the character to the hit point.
                if (characterAgent != null)
                {
                    characterAgent.SetDestination(hit.point);
                }
                else if (character != null)
                {
                    // For testing purposes, you could instantly move the character:
                    character.transform.position = hit.point;
                }
            }
        }
    }
}

