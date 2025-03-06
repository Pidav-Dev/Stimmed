using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StateChange : MonoBehaviour
{
    private static bool occupied; // Control variable to let the user work on one stimulus at time
    
    [Header("Endurance Wear")] // Fields for specific wear
    [SerializeField] private int stimulusAmount = 2; // Describes amount of sensory overload added each stimulusInterval
    [SerializeField] private float stimulusInterval = 1f; // Describes how often the amount of endurance changes
    [SerializeField] private int gestureType;
    
    // Level events
    public UnityEvent<int> interacted; // Called when the stimulus is cleared
    public UnityEvent changePosition; // Called to change the position of the camera to the stimulus
    public UnityEvent returnPosition;  // Called to go back to original position
    public UnityEvent<int> gestureHandler; 
    
    private bool _isActive; // Determine if the stimuli is sensory overloading 
    //private bool _isFocusing; // Determine if the stimulus is being focused on with the camera
    private Renderer _objectRenderer; // Get the renderer of the actual object
    private Color _originalColor; // Get renderer's color
    private int _enduranceAmount = 1; // Initial amount of endurance wear
    private AudioSource _audioSource; // Audio component for stimulus feature
    
    private CameraInteractions _tap; // Input map for interactions

    void Awake()
    {
        _tap = new CameraInteractions(); // Create a new instance of the input map
    }
    
    // Enables Input Actions and subscribe to a behaviour to it when the component is enabled 
    void OnEnable()
    {
        _tap.Camera.Enable();
        _tap.Camera.Tap.performed += OnTap; 
    }

    // Disables Input Actions and unsubscribe from its behaviour to it when the component is disabled 
    void OnDisable()
    {
        _tap.Camera.Tap.performed -= OnTap; 
        _tap.Camera.Disable();
    }

    void Start()
    {
        // Assign the component of the very own GameObject
        _audioSource = GetComponent<AudioSource>();
        _objectRenderer = GetComponent<Renderer>(); 
        // Assign renderer's color to its variable
        if (_objectRenderer != null)
        {
            _originalColor = _objectRenderer.material.color;
        }
        StartCoroutine(ActivateRandomly()); // Start the concurrent routine of the random activation
        StartCoroutine(IncreaseEndurance()); // Start the concurrent routine of the random activation
    }

    void Update()
    {
        // Stops audio if game is on freeze
        if (Time.timeScale == 0f)  _audioSource.Stop();
    }

    // Concurrently activates the stimuli with a random wait time
    private IEnumerator ActivateRandomly()
    {
        // Produces warning but it is needed to allow infinite looping 
        while (true)
        {
            float waitTime = Random.Range(0f, 10f); // Determine a random wait time for the activation 
            // After the determined time the color is changed and the control is given back to the parent
            yield return new WaitForSeconds(waitTime);
            // Stimulus activation
            if (!_isActive) 
            {
                _isActive = true; // Trigger stimulus 
                _enduranceAmount = 1;
                ChangeColor(Color.red); // Make the stimulus visible
                _audioSource.Play(); // Let the user hear the stimulus
            }
        }
    }
    
    // Concurrently wear endurance based on Endurance Wear parameters 
    private IEnumerator IncreaseEndurance()
    {
        // Produces warning, it is needed to allow infinite looping 
        while (true)
        {
            yield return new WaitForSeconds(stimulusInterval); // Before incrementing, it waits for some time
            // Wear endurance only if the stimulus is active and the game is not on freeze
            if (_isActive && Time.timeScale > 0)
            {
                _enduranceAmount += stimulusAmount;
                _enduranceAmount = Math.Clamp(_enduranceAmount, 0, 10);
                interacted?.Invoke(_enduranceAmount); // Invoke event to actually wear endurance
            }
        }
    }

    // Focus on the stimulus or interacts and restore part of endurance with it if already focused
    private void OnTap(InputAction.CallbackContext context)
    {
        // Execute only if isActive, if the user is not working on another stimulus or if the game has not ended
        if (!_isActive || occupied || Time.timeScale == 0) return; 
        
        Vector2 touchPosition;  // Get position to raycast
    
        // Get primary touch position from mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) 
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        } 
        // Get mouse position from PC
        else if (Mouse.current != null) 
        {
            touchPosition = Mouse.current.position.ReadValue();
        }
        // In case the two input methods are not successful
        else
        {
            touchPosition = Vector2.zero;
        }
        
        // Raycast from touch position
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        // True if the ray intersects the caller game object
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == this.gameObject)
        {
            occupied = true; // The user is working with some stimulus and impede other interactions 
            // Interacts with the stimulus and restore endurance  
            changePosition?.Invoke(); // Invokes the event for camera focusing
            gestureHandler?.Invoke(gestureType); // Invokes the event for correct gesture detection
        }
    }

    // Changes renderer's color
    private void ChangeColor(Color newColor)
    {
        if (_objectRenderer) _objectRenderer.material.color = newColor;
    }

    // The user correctly cleared the stimulus, so the endurance needs to be restored 
    public void CorrectlyInteracted()
    {
        _isActive = false; // Allow stimulus to be respawned
        ChangeColor(_originalColor); // Change element's color to communicate better
        _audioSource.Stop(); // Stops stimulus audio when interacted
        interacted?.Invoke(-(_enduranceAmount*stimulusAmount)); // Invokes event for endurance restoration
        returnPosition?.Invoke(); // Invokes event for position returning 
        occupied = false; // The user can interact back with other stimuli
    }
}