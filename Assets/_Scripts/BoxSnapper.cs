using UnityEngine;

public class BoxSnapper : MonoBehaviour
{
    [Header("Box References")]
    public Transform boxA; // The moving box
    public Transform boxS1; // The target box

    [Header("Snapping Settings")]
    public float snapThreshold = 0.5f; // Distance within which snapping occurs
    public Vector3 snapDirection = Vector3.right; // Direction for snapping
    public float snapDistance = 1.0f; // Distance from boxS1 after snapping
    public bool allowResnap = false; // Allow re-snapping after moving away

    private bool isSnapped = false;
    private Rigidbody rb;

    private void Start()
    {
        if (boxA == null || boxS1 == null)
        {
            Debug.LogError("BoxSnapper Error: Assign both boxA and boxS1 in the inspector!");
            enabled = false;
            return;
        }

        // Get Rigidbody (optional) for physics-based movement
        rb = boxA.GetComponent<Rigidbody>();

        Debug.Log("BoxSnapper Initialized.");
    }

    private void Update()
    {
        if (boxA == null || boxS1 == null) return;

        float distance = Vector3.Distance(boxA.position, boxS1.position);
        Debug.Log($"Distance between boxA and boxS1: {distance:F3}");

        // Snap if within threshold and not already snapped
        if (!isSnapped && distance <= snapThreshold)
        {
            SnapBoxA();
        }
        // If re-snapping is enabled and boxA moves away, reset snapping
        else if (allowResnap && isSnapped && distance > snapThreshold)
        {
            Debug.Log("BoxA moved away. Resetting snap.");
            isSnapped = false;
        }
    }

    private void SnapBoxA()
    {
        Vector3 targetPosition = boxS1.position + (snapDirection.normalized * snapDistance);

        Debug.Log($"Snapping boxA to {targetPosition}");

        // Move boxA
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Stop movement
            rb.angularVelocity = Vector3.zero;
            rb.position = targetPosition; // Physics-based movement
        }
        else
        {
            boxA.position = targetPosition; // Direct transformation
        }

        isSnapped = true;
    }
}
