using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class EnduranceManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    public UnityEvent losingCondition; // Event to manage the losing condition
    
    private int _endurance; // Level of endurance that describes character's sensory overload 
    private const int MaxEndurance = 100; // Means character is sensory overloaded and Game Over
    
    // Visual elements for the endurance's progress bar
    private VisualElement _topFill;
    private VisualElement _rightFill;
    private VisualElement _bottomFill;
    private VisualElement _leftFill;
    
    private void Start()
    {
        _endurance = 0; // Endurance starts at zero 
        // Assign reference to actual visual element
        _topFill = uiDocument.rootVisualElement.Q<VisualElement>("top-fill"); 
        _rightFill = uiDocument.rootVisualElement.Q<VisualElement>("right-fill"); 
        _bottomFill = uiDocument.rootVisualElement.Q<VisualElement>("bottom-fill"); 
        _leftFill = uiDocument.rootVisualElement.Q<VisualElement>("left-fill"); 
    }

    private void Update()
    {
        if (_endurance == MaxEndurance) losingCondition?.Invoke(); // When the user maxes out the endurance, it loses
        // Update progress bar each frame
        UpdateProgressBar();
    }

    // Divide endurance's progress in for segments, each assigned to one visual element
    private void UpdateProgressBar()
    {
        // First segment: 0 - 25
        float topAmount = Mathf.Clamp01(Math.Clamp(_endurance, 0, 25)/25f);
        _topFill.transform.scale = new Vector3(topAmount, 1, 1);
        // Second segment: 25 - 50
        float rightAmount = Mathf.Clamp01((Math.Clamp(_endurance, 25, 50)-25)/25f);
        _rightFill.transform.scale = new Vector3(1, rightAmount, 1);
        // Third segment: 50 - 75
        float bottomAmount = Mathf.Clamp01((Math.Clamp(_endurance, 50, 75)-50)/25f);
        _bottomFill.transform.scale = new Vector3(bottomAmount, 1, 1);
        // Fourth segment: 75 - 50
        float leftAmount = Mathf.Clamp01((Math.Clamp(_endurance, 75, 100)-75)/25f);
        _leftFill.transform.scale = new Vector3(1, leftAmount, 1);
    }

    // Getter and setter for the endurance
    public int Endurance
    {
        get => _endurance;
        set => _endurance = Mathf.Clamp(_endurance + value, 0, MaxEndurance);
    }
}
