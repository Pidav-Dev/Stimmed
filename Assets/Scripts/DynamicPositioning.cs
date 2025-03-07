using UnityEngine;

public class DynamicPositioning : MonoBehaviour
{
    [Header("Positioning Settings")] // Fields for positioning and speed settings
    [SerializeField] private float followSpeed = 3f; // Speed in which the camera will follow the stimulus
    [SerializeField] private float frontDistance = 2f; // Distance in which the camera will be place once in position 
    [SerializeField] private float verticalOffset = 0.5f; // Vertical angle in which the camera will be placed once in position

    [Header("Rotation Settings")] // Field for the rotation speed
    [SerializeField] private float rotationSpeed = 5f; // Speed of the rotation while getting in position

    [Header("Return Settings")] // Fields for the return speed and position
    [SerializeField] private float returnSpeed = 4f; // Speed in which the camera will move to get back to position
    [SerializeField] private float positionThreshold = 0.1f; // Threshold behind which determine returning arrival 

    private Vector3 _originalPosition; // Starting position of every movement
    private Quaternion _originalRotation; // Starting rotation of every movement
    private bool _isReturning; // True if the camera is returning back in place at the end of each movement
    private Transform _target; // Target's transform to reach at every movement
    
    private CameraPivot _cameraMovement; // Component to disable camera movement's input when the position is focused

    private void Start()
    {
        // Get component from GameObject 
        _cameraMovement = GetComponent<CameraPivot>();
        // Save starting position and rotation
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        // Skips rotation and position update position if the camera is returning in place 
        if(_isReturning)
        {
            ReturnToOriginalPosition();
            return;
        }

        // Update position and rotation if some target exists 
        if(_target)
        {
            _isReturning = false; // If a stimulus is targeted, the camera needs to reach it not to return in place 
            UpdatePositionAndRotation();
        }
    }

    // Update target's position in order to reach it and starts rotation
    private void UpdatePositionAndRotation()
    {
        Vector3 desiredPosition = CalculateFrontPosition();

        _cameraMovement.enabled = false; // Disables input controls for camera when focused on a stimulus
        
        // Interpolate linearly towards target's position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
        RotateTowardsTarget(); // Starts rotation
    }
    
    // Returns target's front position to give the camera a place to be directed towards to
    private Vector3 CalculateFrontPosition()
    {
        return _target.position + 
               _target.forward * frontDistance + 
               Vector3.up * verticalOffset;
    }
    
    // Determine target's rotation and starts rotating
    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_target.position - transform.position);
        // Interpolate linearly in spherical coordinates towards target's position
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Rotate and place camera in original position and rotation
    private void ReturnToOriginalPosition()
    {
        // Move towards original position
        transform.position = Vector3.Lerp(
            transform.position,
            _originalPosition,
            returnSpeed * Time.deltaTime
        );

        // Rotate towards original position
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _originalRotation,
            returnSpeed * Time.deltaTime
        );

        // Check if the original position is reached among a certain threshold
        if(Vector3.Distance(transform.position, _originalPosition) < positionThreshold)
        {
            _isReturning = false; // Stops returning phase
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            _cameraMovement.enabled = true; // Enables again input controls when back from focus 
        }
    }

    // Sets new target and stops any returning phase
    public void SetNewTarget(Transform newTarget)
    {
        if (_target) return; // Don't allow target shifting while focused
        _target = newTarget;
        _isReturning = false;
    }

    // Starts returning and resets target
    public void StartReturning()
    {
        _isReturning = true;
        _target = null;
    }
}