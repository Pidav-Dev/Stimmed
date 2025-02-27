using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class StateChange : MonoBehaviour
{
    [SerializeField] private int stimulusAmount = 2; 
    [SerializeField] private float stimulusInterval = 1f;
    [SerializeField] private EnduranceManager character;
    private bool _isActive; // Determine if the stimuli is sensory overloading 
    private Renderer _objectRenderer; // Get the renderer of the actual object
    private Color _originalColor; // Get renderer's color
    private int _enduranceAmount = 1; 

    void Start()
    {
        _objectRenderer = GetComponent<Renderer>(); // Assign the Renderer of the actual component 
        // Assign renderer's color to its variable
        if (_objectRenderer != null)
        {
            _originalColor = _objectRenderer.material.color;
        }
        StartCoroutine(ActivateRandomly()); // Start the concurrent routine of the random activation
        StartCoroutine(IncreaseEndurance()); // Start the concurrent routine of the random activation
    }

    // Concurrently activates the stimuli with a random wait time
    private IEnumerator ActivateRandomly()
    {
        // Produces warning but it is needed to allow infinite looping 
        while (true)
        {
            float waitTime = Random.Range(0f, 10f); // Determine a random wait time 
            // After the determined time the color is changed and the control is given back to the parent
            yield return new WaitForSeconds(waitTime);
            if (!_isActive) 
            {
                _isActive = true;
                _enduranceAmount = 1;
                ChangeColor(Color.red);
                Debug.Log(gameObject.name + " Activated (Color: Red)");
            }
        }
    }
    
    private IEnumerator IncreaseEndurance()
    {
        while (true)
        {
            yield return new WaitForSeconds(stimulusInterval); // Before incrementing, it waits for some time
            if (_isActive && Time.timeScale > 0)
            {
                _enduranceAmount += stimulusAmount;
                _enduranceAmount = Math.Clamp(_enduranceAmount, 0, 10);
                character.SetEndurance(_enduranceAmount);
            }
        }
    }

    // Increase endurance once the user clicks down
    void OnMouseDown()
    {
        if (!_isActive || Time.timeScale == 0) return; // Execute only if isActive or the game has not ended
        _isActive = false;
        ChangeColor(_originalColor); // Change element's color
        // Increase universal endurance when clicking any active object
        character.SetEndurance(-(_enduranceAmount*stimulusAmount));
    }

    // Changes renderer's color
    private void ChangeColor(Color newColor)
    {
        // Instead of _objectRenderer != null, too expensive 
        if (_objectRenderer)
        {
            _objectRenderer.material.color = newColor;
        }
    }
}