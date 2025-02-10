using UnityEngine;


public class ClickToMove : MonoBehaviour
{
    [Tooltip("Movement speed in units per second.")]
    public float speed = 3.0f;

    void FixedUpdate()
    {
        // Get the right joystick input from the Oculus Touch Controller.
        // The secondary thumbstick on the right controller provides a Vector2 input.
        Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // If there is any significant input...
        if (joystickInput.sqrMagnitude > 0.01f)
        {
            // Create a movement vector based on the joystick's x and y values.
            // This represents movement in the horizontal plane.
            Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y);
            // Optionally normalize to ensure uniform speed in all directions.
            moveDirection = moveDirection.normalized;

            // Update the cylinder's position in world space.
            transform.position += moveDirection * speed * Time.deltaTime;

            // Rotate the cylinder so that it faces the movement direction.
            if (moveDirection != Vector3.zero)
            {
                // Use Vector3.up to constrain rotation around the Y-axis.
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }

        }
    }
}


