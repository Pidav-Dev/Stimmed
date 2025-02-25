using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] private Transform pivot; // Point around which allow the rotation
    [SerializeField] private float rotationSpeed = 5.0f; // Define the sensibility of the rotation
    [SerializeField] private float minVerticalAngle = 0f;  // Define inferior limit for the vertical rotation
    [SerializeField] private float maxVerticalAngle = 30f; // Define superior limit for the vertical rotation 
    [SerializeField] private float inertiaDamping = 0.95f; // Define the "throw" effect

    private Vector2 lastTouchPosition; // Detect where last input weas
    private Vector2 velocity = Vector2.zero; // Camera velocity used for the "throw"
    private bool isDragging = false; 
    private float currentVerticalAngle = 15f;

    void Start()
    {
        // Set a limited initial angle
        Vector3 initialRotation = transform.eulerAngles;
        currentVerticalAngle = Mathf.Clamp(initialRotation.x, minVerticalAngle, maxVerticalAngle);
        transform.eulerAngles = new Vector3(currentVerticalAngle, initialRotation.y, initialRotation.z);
    }

    void Update()
    {
        Vector2 delta = Vector2.zero; // Each frame the touch differential from the previous frame is resetted

        // Single touch control for iPhone
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            // Setup rotation once touch begins
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position; // Set the last touched position to the actual one
                isDragging = true;
                velocity = Vector2.zero; // When the new drag starts the velocity is set to zero
            }
            // Determine delta during the touch
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                delta = touch.deltaPosition;
            }
            // Set off the rotation once touch ends
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // Mouse control in editor (for debug only) 
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
            isDragging = true;
            velocity = Vector2.zero;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            delta = (Vector2)Input.mousePosition - lastTouchPosition;
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        // Updates velocity
        if (isDragging)
        {
            // The update follows the differential of touch's movement and the delta time of the system
            velocity = delta * rotationSpeed * Time.deltaTime; 
        }
        else
        {
            // Apply damping to speed to simulate inertia
            velocity *= inertiaDamping;
        }

        // Apply rotation based on the gained speed
        RotateCamera(velocity);
    }

    // Rotate the camera based on a given velocity 
    void RotateCamera(Vector2 delta)
    {
        // Define axis rotation
        float rotationX = delta.x * rotationSpeed * Time.deltaTime; // Vertical rotation
        float rotationY = -delta.y * rotationSpeed * Time.deltaTime; // Horizontal rotation 

        // Limitless rotation around Y axis
        transform.RotateAround(pivot.position, Vector3.up, rotationX);

        // Determine new vertical angle 
        float newVerticalAngle = currentVerticalAngle + rotationY;
        currentVerticalAngle = Mathf.Clamp(newVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Set limited vertical rotation 
        transform.eulerAngles = new Vector3(currentVerticalAngle, transform.eulerAngles.y, 0f);
    }
}
