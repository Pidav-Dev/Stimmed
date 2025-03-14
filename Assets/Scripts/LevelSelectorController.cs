using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectorController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    private VisualElement _levelContainer; // Container for level selector
    private Button _selectLevelButton; // Button to enter level selection
    private Button _optionsButton; // Options button 

    // Level buttons
    private Button _commutingButton; 
    private Button _examButton; 
    private Button _preparationButton; 
    private Button _groceryButton; 
    private Button _dateButton; 

    private void Start()
    {
        // Get elements' reference 
        var root = uiDocument.rootVisualElement;
        
        _levelContainer = root.Q<VisualElement>("LevelContainer");
        _selectLevelButton = root.Q<Button>("SelectLevelButton");
        _optionsButton = root.Q<Button>("Options");
        
        _commutingButton = _levelContainer.Q<Button>("Commuting");
        _examButton = _levelContainer.Q<Button>("Exam");
        _preparationButton = _levelContainer.Q<Button>("Preparation");
        _groceryButton = _levelContainer.Q<Button>("Grocery");
        _dateButton = _levelContainer.Q<Button>("Date");

        // Subscribe to behaviours
        _selectLevelButton.clicked += ShowLevelCarousel;
        _optionsButton.clicked += ShowOptionsMenu;
        
        _commutingButton.clicked += CommutingScene;
        _examButton.clicked += SceneNotReady;
        _preparationButton.clicked += SceneNotReady;
        _groceryButton.clicked += SceneNotReady;
        _dateButton.clicked += SceneNotReady;
    }

    // Called when user taps on screen once opened the game
    private void ShowLevelCarousel()
    {
        _levelContainer.style.display = DisplayStyle.Flex; // Shows the level selector
        _optionsButton.style.display = DisplayStyle.Flex; // Shows the options button
        _selectLevelButton.style.display = DisplayStyle.None; // Hide the button just pressed
        _selectLevelButton.clicked -= ShowLevelCarousel; // Unsubscribe from behaviour to avoid conflicts
    }

    // Called when user taps on option button
    private void ShowOptionsMenu()
    {
        Debug.Log("Options menu not available yet");
    }

    // Methods that loads the single scenes
    private void CommutingScene()
    {
        SceneManager.LoadScene("BusScene", LoadSceneMode.Single);
    }

    // Scene for the "not available" levels
    private void SceneNotReady()
    {
        Debug.Log("Scene not available yet");
    }
}
