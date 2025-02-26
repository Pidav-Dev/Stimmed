using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    // Fields for rotation
    [SerializeField] private Transform pivot; // Point around which to rotate
    [SerializeField] private float rotationSpeed; // Rotation sensitivity
    [SerializeField] private float minVerticalAngle;  // Lower limit for vertical rotation
    [SerializeField] private float maxVerticalAngle; // Upper limit for vertical rotation 
    [SerializeField] private float inertiaDamping; // Camera "throw" (inertia) effect
    // Fields for zoom
    [SerializeField] private float minZoomDistance; // Minimum distance from the pivot
    [SerializeField] private float maxZoomDistance; // Maximum distance from the pivot
    [SerializeField] private float zoomSpeed; // Zoom sensitivity
    // Private fields for state variables
    private Vector2 _lastTouchPosition; // Last touch position
    private Vector2 _velocity = Vector2.zero; // Camera velocity for inertia effect
    private bool _isDragging; // Active when user is dragging 
    private float _currentVerticalAngle = 15f;

    void Start()
    { 
        Application.targetFrameRate = 60; // Set frame rate to 60 fps
        // Set an initial clamped angle
        Vector3 initialRotation = transform.eulerAngles; // Returns component's initial angles
        _currentVerticalAngle = Mathf.Clamp(initialRotation.x, minVerticalAngle, maxVerticalAngle); // Limits the currentVerticalAngle to [min, max]
        transform.eulerAngles = new Vector3(_currentVerticalAngle, initialRotation.y, initialRotation.z); // Changes component's angle to the clamped one
    }

    void Update()
    {
        // ------------------------------------------ TOUCH CONTROLS ---------------------------------------------------
        // Handle pinch-to-zoom
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
            return; // Does not account for any touch types
        }
        
        Vector2 delta = Vector2.zero; // Reset delta's movement each frame

        // Handle rotation
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0); // Get the first detected touch
            
            // Update velocity and touch position when touch begins
            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPosition = touch.position; // Save touch position
                _isDragging = true;
                _velocity = Vector2.zero; // Stops any previous movements
            }
            // Update touch's delta when the user is dragging
            else if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                // Calculate movement delta
                delta = touch.deltaPosition;
            }
            // Disables dragging when the touch ends
            else if (touch.phase == TouchPhase.Ended)
            {
                _isDragging = false;
            }
        }
        
        // ------------------------------------------ MOUSE CONTROLS (Debugging only)  ---------------------------------
        // Handle pinch-to-zoom using the mouse wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            float scroll = Input.mouseScrollDelta.y;
            float adjustedScroll = scroll * zoomSpeed * Time.deltaTime;
            NewCameraDistance(adjustedScroll);
        }
        
        // Handle rotation using mouse 
        if (Input.GetMouseButtonDown(0))
        {
            _lastTouchPosition = Input.mousePosition;
            _isDragging = true;
            _velocity = Vector2.zero;
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            delta = (Vector2)Input.mousePosition - _lastTouchPosition;
            _lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
        // -------------------------------------------------------------------------------------------------------------
        
        // While dragging, velocity is updated based on delta and rotation speed
        if (_isDragging)
        {
            _velocity = delta * (rotationSpeed * Time.deltaTime);
        }
        // While not dragging, the velocity gets damping effects added to simulate inertia
        else
        {
            _velocity *= inertiaDamping;
        }

        // Apply rotation based on accumulated velocity
        RotateCamera(_velocity);
    }

    // Rotate the camera based on a given movement delta
    void RotateCamera(Vector2 delta)
    {
        float rotationX = delta.x * rotationSpeed * Time.deltaTime; // Rotation around the Y-axis (horizontal)
        float rotationY = -delta.y * rotationSpeed * Time.deltaTime; // Vertical rotation

        // Rotate the camera around the pivot along the Y-axis
        transform.RotateAround(pivot.position, Vector3.up, rotationX);

        // Calculate and clamp the new vertical angle
        float newVerticalAngle = _currentVerticalAngle + rotationY;
        _currentVerticalAngle = Mathf.Clamp(newVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Set the clamped vertical rotation
        transform.eulerAngles = new Vector3(_currentVerticalAngle, transform.eulerAngles.y, 0f);
    }

    // Handle pinch-to-zoom
    private void HandlePinchZoom()
    {
        // Gets the first two touches on screen
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
        Vector3 direction = (transform.position - pivot.position).normalized; // Gets distance's direction
        float distance = Vector3.Distance(transform.position, pivot.position); // Gets distance between component and pivot
        float newDistance = distance + deltaMagnitudeDiff * zoomSpeed * Time.deltaTime; // Determine the distance based on new touches
        newDistance = Mathf.Clamp(newDistance, minZoomDistance, maxZoomDistance); // Clamps the distance in [min, max]

        // Update camera position along the same direction
        transform.position = pivot.position + direction * newDistance;
    }
    
    // ------------------------------------------ MOUSE PINCH-TO-ZOOM (Debugging only)  --------------------------------
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
