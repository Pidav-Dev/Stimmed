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
    
    private void Start()
    {
        endurance = 0; 
        _enduranceLabel = uiDocument.rootVisualElement.Q<Label>("Endurance"); // Find the UI element in UXML file and attach to it
    }

    private void Update()
    {
        // Update label
        _enduranceLabel.text = endurance.ToString();
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
}