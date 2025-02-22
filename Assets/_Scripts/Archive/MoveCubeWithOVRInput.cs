using UnityEngine;

public class MoveCubeWithOVRInput : MonoBehaviour
{
// Reference to the cube to move
    public GameObject cube;

    // Reference to the tabletop (cylinder)
    public GameObject tableTop;

    // Line Renderer for ray visualization (optional)
    private LineRenderer lineRenderer;

    void Start()
    {
        // Setup Line Renderer for ray visualization (optional)
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Basic material
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // Update controller position and rotation
        transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Check if the right controller's trigger is pressed
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            // Cast a ray from the controller's position and direction
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            // Update Line Renderer to show the ray
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 10f);

            // Perform raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray intersects the tabletop
                if (hit.collider.gameObject == tableTop)
                {
                    // Move the cube to the intersection point
                    cube.transform.position = hit.point;

                    // Adjust cube's Y position to sit on the table (half its height above the hit point)
                    cube.transform.position = new Vector3(hit.point.x, hit.point.y + cube.transform.localScale.y / 2, hit.point.z);
                }
            }
        }
        else
        {
            // Collapse Line Renderer when trigger is not pressed
            lineRenderer.SetPosition(1, transform.position);
        }
    }
}
