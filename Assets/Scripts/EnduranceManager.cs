using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class EnduranceManager : MonoBehaviour
{
    public static EnduranceManager Instance; // Used to reference the endurance in other scripts
    [SerializeField] private int endurance; // Level of endurance that describes character's sensory overload 
    [SerializeField] private UIDocument uiDocument;
    private const int MaxEndurance = 100; // Means character is sensory overloaded and you lost
    private Label _enduranceLabel; // Label where it will be attached 
    private VisualElement _topFill;
    private VisualElement _rightFill;
    private VisualElement _bottomFill;
    private VisualElement _leftFill;
    
    private void Start()
    {
        endurance = 0; 
        _enduranceLabel = uiDocument.rootVisualElement.Q<Label>("Endurance"); // Find the UI element in UXML file and attach to it
        _topFill = uiDocument.rootVisualElement.Q<VisualElement>("top-fill"); // Name your fill element in UI Builder
        _rightFill = uiDocument.rootVisualElement.Q<VisualElement>("right-fill"); // Name your fill element in UI Builder
        _bottomFill = uiDocument.rootVisualElement.Q<VisualElement>("bottom-fill"); // Name your fill element in UI Builder
        _leftFill = uiDocument.rootVisualElement.Q<VisualElement>("left-fill"); // Name your fill element in UI Builder
    }

    private void Update()
    {
        // Update label
        _enduranceLabel.text = endurance.ToString();
        UpdateProgressBar();
    }

    // Returns endurance following information hiding principles
    public int GetEndurance()
    {
        return endurance;
    }

    public void SetEndurance(int amount)
    {
        endurance = Mathf.Clamp(endurance + amount, 0, MaxEndurance);
    }

    private void UpdateProgressBar()
    {
        float topAmount = Mathf.Clamp01(Math.Clamp(endurance, 0, MaxEndurance)/100f);
        _topFill.transform.scale = new Vector3(topAmount, 1, 1);
        _leftFill.transform.scale = new Vector3(1, topAmount, 1);
        float bottomAmount = Mathf.Clamp01(Math.Clamp(endurance, 0, MaxEndurance)/100f);
        _bottomFill.transform.scale = new Vector3(bottomAmount, 1, 1);
        _rightFill.transform.scale = new Vector3(1, bottomAmount, 1);
    }
}
