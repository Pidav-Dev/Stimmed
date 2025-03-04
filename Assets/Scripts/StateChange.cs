using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.Events;
// ReSharper disable IteratorNeverReturns

public class StateChange : MonoBehaviour
{
    [Header("Endurance Wear")] // Fields for specific wear
    [SerializeField] private int stimulusAmount = 2; // Describes amount of sensory overload added each stimulusInterval
    [SerializeField] private float stimulusInterval = 1f; // Describes how often the amount of endurance changes
    
    // Level events
    public UnityEvent<int> interacted; // Called when the stimulus is cleared
    public UnityEvent changePosition; // Called to change the position of the camera to the stimulus
    public UnityEvent returnPosition;  // Called to go back to original position
    
    private bool _isActive; // Determine if the stimuli is sensory overloading 
    private bool _isFocusing; // Determine if the stimulus is being focused on with the camera
    private Renderer _objectRenderer; // Get the renderer of the actual object
    private Color _originalColor; // Get renderer's color
    private int _enduranceAmount = 1; // Initial amount of endurance wear
    private AudioSource _audioSource; // Audio component for stimulus feature

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
        if (Time.timeScale == 0f)
        {
            _audioSource.Stop();
        }
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
                _isActive = true;
                _isFocusing = false;
                _enduranceAmount = 1;
                ChangeColor(Color.red);
                _audioSource.Play();
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
    void OnMouseDown()
    {
        if (!_isActive || Time.timeScale == 0) return; // Execute only if isActive or the game has not ended
        // Interacts with the stimulus and restore endurance  
        if (_isFocusing)
        {
            _isActive = false;
            ChangeColor(_originalColor); // Change element's color to communicate better
            _audioSource.Stop(); // Stops stimulus audio when interacted
            interacted?.Invoke(-(_enduranceAmount*stimulusAmount)); // Invokes event for endurance restoration
            returnPosition?.Invoke(); // Invokes event for position returning 
        }
        // Focus the camera on the stimulus when firstly interacted
        else
        {
            _isFocusing = true; 
            changePosition?.Invoke(); // Invokes the event for camera focusing
        }
    }

    // Changes renderer's color
    private void ChangeColor(Color newColor)
    {
        if (_objectRenderer)
        {
            _objectRenderer.material.color = newColor;
        }
    }
}