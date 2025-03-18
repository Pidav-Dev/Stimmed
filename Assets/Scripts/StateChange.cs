using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StateChange : MonoBehaviour
{
    private static bool occupied; // Control variable to let the user work on one stimulus at time
    
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    [Header("Endurance Wear")] // Fields for specific wear
    [SerializeField] private int stimulusAmount = 2; // Describes amount of sensory overload added each stimulusInterval
    [SerializeField] private float stimulusInterval = 1f; // Describes how often the amount of endurance changes
    [SerializeField] private Texture2D gestureIcon;
    
    // Level events
    public UnityEvent<int> interacted; // Called when the stimulus is cleared
    public UnityEvent changePosition; // Called to change the position of the camera to the stimulus
    public UnityEvent returnPosition;  // Called to go back to original position
    public UnityEvent<int> gestureHandler; 
    
    private bool _isActive; // Determine if the stimuli is sensory overloading 
    private int _enduranceAmount = 1; // Initial amount of endurance wear
    private AudioSource _audioSource; // Audio component for stimulus feature
    private VisualElement _gestureInfo; // UI gesture icon reference
    
    private CameraInteractions _tap; // Input map for interactions

    void Awake()
    {
        // Create a new instance of the input map
        _tap = new CameraInteractions(); 
        // Get elements' reference 
        var root = uiDocument.rootVisualElement;
        _gestureInfo = root.Q<VisualElement>("GestureInfo");
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
        ChangeOutline(false);
        StartCoroutine(ActivateRandomly()); // Start the concurrent routine of the random activation
        StartCoroutine(IncreaseEndurance()); // Start the concurrent routine of the random activation
    }

    void Update()
    {
        // Stops audio if game is on freeze
        if (Time.timeScale == 0f) _audioSource.Pause();
        else _audioSource.UnPause();
    }

    // Concurrently activates the stimuli with a random wait time
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator ActivateRandomly()
    {
        // Produces warning but it is needed to allow infinite looping 
        while (true)
        {
            while (_isActive)
            {
                yield return null; // Yield each frame to prevent freezing
            }
            // Longer wait if visible, Shorter wait if outside FOV
            float waitTime = IsVisible() ? Random.Range(15f, 20f) : Random.Range(2f, 10f); 

            // After the determined time the color is changed and the control is given back to the parent
            yield return new WaitForSeconds(waitTime);
            // Stimulus activation
            _isActive = true; // Trigger stimulus 
            _enduranceAmount = 1;
            ChangeOutline(true); // Make the stimulus visible
            _audioSource.Play(); // Let the user hear the stimulus
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
                interacted?.Invoke(stimulusAmount); // Invoke event to actually wear endurance
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
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            occupied = true; // The user is working with some stimulus and impede other interactions 
            // Interacts with the stimulus
            _gestureInfo.style.backgroundImage = gestureIcon; // Change the gesture icon to the mapped one
            changePosition?.Invoke(); // Invokes the event for camera focusing
            gestureHandler?.Invoke(0); // Invokes the event for correct gesture detection
        }
    }

    // Changes renderer's outline based on activeness
    private void ChangeOutline(bool active)
    {
        return; 
    }

    // Invoked by event when the user correctly cleared the stimulus, so the endurance needs to be restored 
    public void CorrectlyInteracted()
    {
        _isActive = false; // Allow stimulus to be respawned
        ChangeOutline(false); // Change element's color to communicate better
        _audioSource.Stop(); // Stops stimulus audio when interacted
        interacted?.Invoke(-_enduranceAmount); // Invokes event for endurance restoration
        returnPosition?.Invoke(); // Invokes event for position returning 
        _gestureInfo.style.backgroundImage = null; // Change the icon back to null
        occupied = false; // The user can interact back with other stimuli
    }
    
    // Checks if the stimulus enters the FOV
    private bool IsVisible()
    {
        if (Camera.main == null) return false;
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPoint is { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 };
    }
}