using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    // UI elements
    private VisualElement _tutorialPanel;
    private VisualElement _gestureContainer;
    private Label _firstLabel; 
    private Label _secondLabel;
    private Button _continueButton;

    private bool _tutorialToggled; // Describe tutorial's progression
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0; // Time frozen to not let start the game before the Tutorial
        AudioListener.pause = true;
        
        // Get elements' reference
        var root = uiDocument.rootVisualElement;
        _tutorialPanel = root.Q<VisualElement>("TutorialPanel");
        _gestureContainer = _tutorialPanel.Q<VisualElement>("GestureContainer");
        _firstLabel = _tutorialPanel.Q<Label>("FirstLabel");
        _secondLabel = _tutorialPanel.Q<Label>("SecondLabel");
        _continueButton = _tutorialPanel.Q<Button>("ContinueButton");
        _continueButton.clicked += ToggleTutorial;
    }
    
    // GestureContainer

    // Changes panel on screen according to tutorial's state
    private void ToggleTutorial()
    {
        // Tutorial is  on first label
        if (!_tutorialToggled)
        {
            _tutorialToggled = true;
            _firstLabel.style.display = DisplayStyle.None;
            _secondLabel.style.display = DisplayStyle.Flex;
            _gestureContainer.style.display = DisplayStyle.Flex;
            return; 
        }
        // Tutorial is on second label 
        _firstLabel.style.display = DisplayStyle.None;
        _secondLabel.style.display = DisplayStyle.None;
        _tutorialPanel.style.display = DisplayStyle.None;
        _gestureContainer.style.display = DisplayStyle.None;
        _continueButton.style.display = DisplayStyle.None;
        Time.timeScale = 1; // Tutorial ended, the game is being played
        AudioListener.pause = false;
    }
}
