using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] private Transform pivot; // Point around which rotate
    [SerializeField] private float rotationSpeed = 5.0f; // Rotation velocity (less == more stiff)
    [SerializeField] private float minVerticalAngle = 0f;  // Inferior limit angle
    [SerializeField] private float maxVerticalAngle = 30f; // Superior limit angle

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    [SerializeField] private float currentVerticalAngle = 15f; // Initialized angle

    void Start()
    {
        // Be sure that camera starts with correct angle
        Vector3 initialRotation = transform.eulerAngles;
        currentVerticalAngle = Mathf.Clamp(initialRotation.x, minVerticalAngle, maxVerticalAngle);
        transform.eulerAngles = new Vector3(currentVerticalAngle, initialRotation.y, initialRotation.z);
    }

    void Update()
    {
        // Touch control for the iPhone
        // Allow only single touch for panning
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            // Initialize touch once the touch begins
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position; // keep track of touch position each frame
                isDragging = true; 
            }
            // Act once the touch position changes and while dragging
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.deltaPosition; // Determine the difference between last frame's position and actual's
                RotateCamera(delta); // Rotate of delta's angle
            }
            // Deinitialize touch once the touch ends
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // Mouse control for debugging (same of touch)
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            lastTouchPosition = Input.mousePosition;
            RotateCamera(delta);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // Rotate the camera of delta's angle
    void RotateCamera(Vector2 delta)
    {
        // Determine the coordinates of rotation
        float rotationX = delta.x * rotationSpeed * Time.deltaTime;
        float rotationY = -delta.y * rotationSpeed * Time.deltaTime;

        // Rotate aroundo Y axis without limits
        transform.RotateAround(pivot.position, Vector3.up, rotationX);

        // Determine new vertical angle
        float newVerticalAngle = currentVerticalAngle + rotationY;
        currentVerticalAngle = Mathf.Clamp(newVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Rotate vertically with limits 
        transform.eulerAngles = new Vector3(currentVerticalAngle, transform.eulerAngles.y, 0f);
    }
}
