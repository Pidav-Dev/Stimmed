using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    
    [Header("Camera Rotation")] // Fields for rotation
    [SerializeField] private Transform pivot; // Point around which to rotate
    [SerializeField] private float rotationSpeed; // Rotation sensitivity
    [SerializeField] private float minVerticalAngle;  // Lower limit for vertical rotation
    [SerializeField] private float maxVerticalAngle; // Upper limit for vertical rotation 
    [SerializeField] private float inertiaDamping; // Camera "throw" (inertia) effect
    
    // Private fields for state variables
    private Vector2 _velocity = Vector2.zero; // Camera velocity for inertia effect
    private float _currentVerticalAngle = 15f;
    private Vector2 _delta; // Variable for movement detection

    // Enables Input Actions when the component is enabled 
    void OnEnable()
    {
        move.action.Enable();
    }

    // Disables Input Actions when the component is disabled 
    void OnDisable()
    {
        move.action.Disable();
    }
    
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
        _delta = move.action.ReadValue<Vector2>(); // Get input values in form of Vector2
        
        // While dragging, velocity is updated based on delta and rotation speed
        if (_delta != Vector2.zero)
        {
            _velocity = _delta * (rotationSpeed * Time.deltaTime);
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
}
