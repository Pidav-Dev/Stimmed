using UnityEngine;
using System.Collections;

public class StateChange : MonoBehaviour
{
    private bool _isActive; // Determine if the stimuli is sensory overloading 
    private Renderer _objectRenderer; // Get the renderer of the actual object
    private Color _originalColor; // Get renderer's color
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _objectRenderer = GetComponent<Renderer>(); // Assign the Renderer of the actual component 
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        // Assign renderer's color to its variable
        if (_objectRenderer != null)
        {
            _originalColor = _objectRenderer.material.color;
        }
        StartCoroutine(ActivateRandomly()); // Start the concurrent routine of the random activation
    }

    // Concurrently activates the stimuli with a random wait time
    IEnumerator ActivateRandomly()
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
                ChangeColor(Color.red);
                _meshRenderer.enabled = true;
                Debug.Log(gameObject.name + " Activated (Color: Red)");
            }
        }
    }

    // Increase endurance once the user clicks down
    void OnMouseDown()
    {
        if (!_isActive) return; 
        // Execute only if isActive 
        _isActive = false;
        _meshRenderer.enabled = false;
        ChangeColor(_originalColor); // Change element's color
        // Increase universal endurance when clicking any active object
        if (EnduranceManager.Instance != null)
        {
            Debug.Log("Before Reduction: " + EnduranceManager.Instance.GetEndurance());
            EnduranceManager.Instance.IncreaseEnduranceByAmount(10); // Integer reduction
            Debug.Log("After Reduction: " + EnduranceManager.Instance.GetEndurance());
        }
        else
        {
            Debug.LogError("EnduranceManager instance is missing from the scene!");
        }
        Debug.Log(gameObject.name + " Deactivated (Color: Original)");
    }

    // Changes renderer's color
    void ChangeColor(Color newColor)
    {
        // Instead of _objectRenderer != null, too expensive 
        if (_objectRenderer)
        {
            _objectRenderer.material.color = newColor;
        }
    }
}