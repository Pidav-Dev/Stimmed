using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] private Transform pivot; // Point around which to rotate
    [SerializeField] private float rotationSpeed = 5.0f; // Rotation sensitivity
    [SerializeField] private float minVerticalAngle = 0f;  // Lower limit for vertical rotation
    [SerializeField] private float maxVerticalAngle = 30f; // Upper limit for vertical rotation 
    [SerializeField] private float inertiaDamping = 0.95f; // Camera "throw" (inertia) effect

    // Fields for zoom
    [SerializeField] private float minZoomDistance = 2f;   // Minimum distance from the pivot
    [SerializeField] private float maxZoomDistance = 15f;    // Maximum distance from the pivot
    [SerializeField] private float zoomSpeed = 0.5f;         // Zoom sensitivity

    private Vector2 lastTouchPosition; // Last touch/mouse position
    private Vector2 velocity = Vector2.zero; // Camera velocity for inertia effect
    private bool isDragging = false; 
    private float currentVerticalAngle = 15f;

    void Start()
    {
        // Set frame rate to 60 fps
        Application.targetFrameRate = 60;
        // Set an initial clamped angle
        Vector3 initialRotation = transform.eulerAngles;
        currentVerticalAngle = Mathf.Clamp(initialRotation.x, minVerticalAngle, maxVerticalAngle);
        transform.eulerAngles = new Vector3(currentVerticalAngle, initialRotation.y, initialRotation.z);
    }

    void Update()
    {
        // Handle pinch-to-zoom on touch devices
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
            return;
        }
        
        // Handle zoom using the mouse wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            float scroll = Input.mouseScrollDelta.y;
            float adjustedScroll = scroll * zoomSpeed * Time.deltaTime;
            NewCameraDistance(adjustedScroll);
        }
        
        Vector2 delta = Vector2.zero; // Reset the movement delta each frame

        // Single touch control (iPhone)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                // On drag start: save position and reset velocity
                lastTouchPosition = touch.position;
                isDragging = true;
                velocity = Vector2.zero;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                // Calculate movement delta
                delta = touch.deltaPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // Mouse control in editor (for debugging)
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
        
        // Update velocity based on movement
        if (isDragging)
        {
            velocity = delta * rotationSpeed * Time.deltaTime;
        }
        else
        {
            // Apply damping to simulate inertia
            velocity *= inertiaDamping;
        }

        // Apply rotation based on accumulated velocity
        RotateCamera(velocity);
    }

    // Rotate the camera based on a given movement delta
    void RotateCamera(Vector2 delta)
    {
        float rotationX = delta.x * rotationSpeed * Time.deltaTime; // Rotation around the Y-axis (horizontal)
        float rotationY = -delta.y * rotationSpeed * Time.deltaTime; // Vertical rotation

        // Rotate the camera around the pivot along the Y-axis
        transform.RotateAround(pivot.position, Vector3.up, rotationX);

        // Calculate and clamp the new vertical angle
        float newVerticalAngle = currentVerticalAngle + rotationY;
        currentVerticalAngle = Mathf.Clamp(newVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Set the clamped vertical rotation
        transform.eulerAngles = new Vector3(currentVerticalAngle, transform.eulerAngles.y, 0f);
    }

    // Handle pinch-to-zoom for touch devices
    private void HandlePinchZoom()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Calculate previous positions of the touches
        Vector2 prevTouchZeroPos = touchZero.position - touchZero.deltaPosition;
        Vector2 prevTouchOnePos = touchOne.position - touchOne.deltaPosition;

        // Calculate the distance between touches in previous and current positions
        float prevTouchDeltaMag = (prevTouchZeroPos - prevTouchOnePos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Determine the change in distance
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // Calculate the direction from the pivot to the camera
        Vector3 direction = (transform.position - pivot.position).normalized;
        float distance = Vector3.Distance(transform.position, pivot.position);
        float newDistance = distance + deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;
        newDistance = Mathf.Clamp(newDistance, minZoomDistance, maxZoomDistance);

        // Update the camera position along the same direction
        transform.position = pivot.position + direction * newDistance;
    }

    // Handle zoom using the mouse wheel
    private void NewCameraDistance(float adjustedScroll)
    {
        // Calculate the direction from the pivot to the camera
        Vector3 direction = (transform.position - pivot.position).normalized;
        float distance = Vector3.Distance(transform.position, pivot.position);
        float newDistance = distance + adjustedScroll;
        newDistance = Mathf.Clamp(newDistance, minZoomDistance, maxZoomDistance);

        // Update the camera position
        transform.position = pivot.position + direction * newDistance;
    }
}
