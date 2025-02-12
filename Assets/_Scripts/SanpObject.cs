using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class SnapObject : MonoBehaviour
    {
        [Header("Snap Settings")]
        [Tooltip("The target socket’s snap point (a Transform placed at the desired snap position and rotation).")]
        public Transform snapSocket;
        
        [Tooltip("Distance threshold for snapping.")]
        public float snapDistance = 0.2f;
        
        [Tooltip("Time (in seconds) to smoothly move into snap position. Set to 0 for instant snapping.")]
        public float snapDuration = 0f;

    //     private XRGrabInteractable grabInteractable;

    //     private void Awake()
    //     {
    //         // Ensure the XRGrabInteractable component is on this GameObject.
    //         grabInteractable = GetComponent<XRGrabInteractable>();
    //         if(grabInteractable == null)
    //         {
    //             Debug.LogError("XRGrabInteractable component not found on " + gameObject.name);
    //         }
    //     }

    //     private void OnEnable()
    //     {
    //         // Listen for the event when the object is released.
    //         grabInteractable.selectExited.AddListener(OnRelease);
    //     }

    //     private void OnDisable()
    //     {
    //         grabInteractable.selectExited.RemoveListener(OnRelease);
    //     }

    //     private void OnRelease(SelectExitEventArgs args)
    //     {
    //         // Check that a socket has been assigned.
    //         if (snapSocket == null) return;
            
    //         // If the object is within the snapDistance of the socket’s position, snap it.
    //         float distance = Vector3.Distance(transform.position, snapSocket.position);
    //         if (distance <= snapDistance)
    //         {
    //             SnapToSocket();
    //         }
    //     }

    //     private void SnapToSocket()
    //     {
    //         // Optionally, you can add smooth interpolation. For simplicity, we show an instant snap.
    //         if (snapDuration <= 0f)
    //         {
    //             transform.position = snapSocket.position;
    //             transform.rotation = snapSocket.rotation;
    //         }
    //         else
    //         {
    //             // If you prefer smooth snapping, you could start a coroutine for a smooth transition.
    //             StartCoroutine(SmoothSnap());
    //         }
    //     }

    //     private System.Collections.IEnumerator SmoothSnap()
    //     {
    //         Vector3 startPos = transform.position;
    //         Quaternion startRot = transform.rotation;
    //         float elapsed = 0f;

    //         while (elapsed < snapDuration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = Mathf.Clamp01(elapsed / snapDuration);
    //             transform.position = Vector3.Lerp(startPos, snapSocket.position, t);
    //             transform.rotation = Quaternion.Slerp(startRot, snapSocket.rotation, t);
    //             yield return null;
    //         }
            
    //         // Ensure final alignment
    //         transform.position = snapSocket.position;
    //         transform.rotation = snapSocket.rotation;
    //     }
     }
}