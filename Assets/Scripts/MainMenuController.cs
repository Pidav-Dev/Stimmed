using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    [SerializeField] private AudioMixer audioMixer;
    
    private VisualElement _levelContainer; // Container for level selector
    private VisualElement _optionsContainer; // Container for options 
    
    private Button _selectLevelButton; // Button to enter level selection
    private Button _optionsButton; // Options button 
    private Button _backButton; // Back button 

    // Level buttons
    private Button _commutingButton; 
    private Button _examButton; 
    private Button _preparationButton; 
    private Button _groceryButton; 
    private Button _dateButton; 
    private Slider _soundSlider;

    private void Start()
    {
        // Get elements' reference 
        var root = uiDocument.rootVisualElement;
        
        // Containers
        _levelContainer = root.Q<VisualElement>("LevelContainer");
        _optionsContainer = root.Q<VisualElement>("OptionsContainer");
        
        // Control Buttons
        _selectLevelButton = root.Q<Button>("SelectLevelButton");
        _optionsButton = root.Q<Button>("OptionsButton");
        _backButton = root.Q<Button>("BackButton");
        var soundContainer = _optionsContainer.Q<VisualElement>("AmbientContainer");
        _soundSlider = soundContainer.Q<Slider>("SoundSlider");
        Load();
        SetAudio(_soundSlider.value);
        // Changes mixer volume each time slider changes
        _soundSlider.RegisterValueChangedCallback(v =>
        {
            var newValue = v.newValue;
            SetAudio(newValue);
            
        });
        
        // Level Buttons
        _commutingButton = _levelContainer.Q<Button>("Commuting");
        _examButton = _levelContainer.Q<Button>("Exam");
        _preparationButton = _levelContainer.Q<Button>("Preparation");
        _groceryButton = _levelContainer.Q<Button>("Grocery");
        _dateButton = _levelContainer.Q<Button>("Date");

        // Control Buttons' behaviours
        _selectLevelButton.clicked += ShowLevelCarousel;
        _optionsButton.clicked += ShowOptionsMenu;
        _backButton.clicked += HideOptionsMenu;
        
        // Level Buttons' behaviours
        _commutingButton.clicked += CommutingScene;
        _examButton.clicked += SceneNotReady;
        _preparationButton.clicked += SceneNotReady;
        _groceryButton.clicked += SceneNotReady;
        _dateButton.clicked += SceneNotReady;
    }

    // Called when user taps on screen once opened the game
    private void ShowLevelCarousel()
    {
        _levelContainer.style.display = DisplayStyle.Flex; // Show the level selector
        _optionsButton.style.display = DisplayStyle.Flex; // Show the options button
        _selectLevelButton.style.display = DisplayStyle.None; // Hide the button just pressed
    }

    // Called when user taps on option button
    private void ShowOptionsMenu()
    {
        _levelContainer.style.display = DisplayStyle.None; // Hide the level selector
        _optionsButton.style.display = DisplayStyle.None; // Hide the options button
        _optionsContainer.style.display = DisplayStyle.Flex; // Show the options container
        _backButton.style.display = DisplayStyle.Flex; // Show the back button
    }

    // Called when user taps on back button in options menu
    private void HideOptionsMenu()
    {
        _levelContainer.style.display = DisplayStyle.Flex; // Hide the level selector
        _optionsButton.style.display = DisplayStyle.Flex; // Hide the options button
        _optionsContainer.style.display = DisplayStyle.None; // Show the options container
        _backButton.style.display = DisplayStyle.None; // Hide the back button
    }

    // Methods that loads the single scenes
    private void CommutingScene()
    {
        SceneManager.LoadScene("Scenes/BusLevel/Intro", LoadSceneMode.Single);
    }

    // Scene for the "not available" levels
    private void SceneNotReady()
    {
        Debug.Log("Scene not available yet");
    }

    private void SetAudio(float volume)
    {
        audioMixer.SetFloat("Ambient", Mathf.Log10(volume)*20); // Changes audio once exiting options
        audioMixer.SetFloat("Stimuli", Mathf.Log10(volume)*20); // Changes audio once exiting options
        Save(); 
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("Ambient", _soundSlider.value);
        PlayerPrefs.Save(); 
    }

    private void Load()
    {
        _soundSlider.value = PlayerPrefs.GetFloat("Ambient", _soundSlider.value);
    }
}
