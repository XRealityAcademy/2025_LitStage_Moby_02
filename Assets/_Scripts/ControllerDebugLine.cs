using UnityEngine;

public class ControllerDebugLine : MonoBehaviour
{
    private LineRenderer debugLine;
    
    [Tooltip("Maximum distance of the debug ray")]
    public float maxDistance = 100f;

    void Awake()
    {
        // Add a LineRenderer component to this GameObject (i.e. your controller)
        debugLine = gameObject.AddComponent<LineRenderer>();
        
        // Set up the LineRenderer properties.
        // Use an unlit shader so the line's color is not affected by lighting.
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = Color.green;
        debugLine.material = lineMaterial;
        
        // Set the width of the line.
        debugLine.startWidth = 0.01f;
        debugLine.endWidth = 0.01f;
        
        // We only need 2 points (start and end).
        debugLine.positionCount = 2;
    }

    void Update()
    {
        // Use the controller's position and forward direction.
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * maxDistance;
        
        // Update the positions of the LineRenderer.
        debugLine.SetPosition(0, start);
        debugLine.SetPosition(1, end);
    }
}
