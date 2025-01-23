using UnityEngine;

public class FollowHeadMovement : MonoBehaviour
{
    public Transform headTransform; // Reference to the head's transform (e.g., Camera in VR/AR).
    public Vector3 offset = new Vector3(0, -0.2f, 0.5f); // Offset from the head position.
    public float smoothTime = 0.3f; // Smoothing time for movement.
    private Vector3 velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (headTransform == null)
        {
            // Automatically find the main camera (head) if not assigned.
            headTransform = Camera.main.transform;
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if (headTransform != null)
        {
            // Calculate the desired position based on head position and offset.
            Vector3 targetPosition = headTransform.position + headTransform.forward * offset.z +
                                     headTransform.up * offset.y +
                                     headTransform.right * offset.x;
            // Smoothly move the UI to the target position.
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            // Align the UI to face the head.
            transform.rotation = Quaternion.LookRotation(transform.position - headTransform.position, Vector3.up);
        }
    }
}
