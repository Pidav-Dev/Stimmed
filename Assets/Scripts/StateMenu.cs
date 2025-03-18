using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StateMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    // Localization strings
    [SerializeField] private LocalizedString pausedText;
    [SerializeField] private LocalizedString gameOverText;
    [SerializeField] private LocalizedString levelClearedText;
    [SerializeField] private LocalizedString resumeText;
    [SerializeField] private LocalizedString nextLevelText;

    // UI elements
    private Button _pauseButton;
    private VisualElement _overlayPanel;
    private Label _label;
    private Button _playButton;
    private Button _replayButton;
    private Button _mainMenuButton;
    
    private void Awake()
    {
        // Get elements' reference 
        var root = uiDocument.rootVisualElement;
        
        // Get base UI elements
        _pauseButton = root.Q<Button>("Pause");
        _overlayPanel = root.Q<VisualElement>("OverlayPanel");
        _label = _overlayPanel.Q<Label>("Label");
        _playButton = _overlayPanel.Q<Button>("PlayButton");
        _replayButton = _overlayPanel.Q<Button>("ReplayButton");
        _mainMenuButton = _overlayPanel.Q<Button>("MainMenuButton");
        
        // Add event listeners
        _pauseButton.clicked += TogglePause;
        _replayButton.clicked += ReplayGame;
        _mainMenuButton.clicked += MainMenuGame;

        // Setup localization listeners
        pausedText.StringChanged += UpdateLabelText;
        gameOverText.StringChanged += UpdateLabelText;
        levelClearedText.StringChanged += UpdateLabelText;
        resumeText.StringChanged += UpdatePlayButtonText;
        nextLevelText.StringChanged += UpdatePlayButtonText;
    }

    // Called when the user taps on pause button or resume button after pause
    private void TogglePause()
    {
        // Pause or resume the game based on actual state
        var isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        AudioListener.pause = !isPaused; // Pause ambient sounds based on pause state
        
        // Update localized texts
        if (!isPaused)
        {
            pausedText.RefreshString();
            resumeText.RefreshString();
            _playButton.style.display = DisplayStyle.Flex;
            _playButton.clicked -= TogglePause;
            _playButton.clicked += TogglePause;
        }
        else
        {
            _playButton.style.display = DisplayStyle.None;
        }
        
        // Show the Overlay Panel based on actual state
        _overlayPanel.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
    }
    
    // Called when user taps on replay button in Overlay Panel
    private void ReplayGame()
    {
        Time.timeScale = 1f; // Ensure time is unpaused
        SceneManager.LoadScene("BusScene", LoadSceneMode.Single); // Reload actual scene
    }

    // Called when user taps on main menu button in Overlay Panel
    private void MainMenuGame()
    {
        Time.timeScale = 1f; // Ensure time is unpaused
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); // Load Main Menu scene
    }

    // Invoked by event when the user loses
    public void GameOver()
    {
        Time.timeScale = 0f; // Pause the game 
        gameOverText.RefreshString(); // Show correct localized label
        _playButton.style.display = DisplayStyle.None; // Remove Resume button
        _replayButton.style.display = DisplayStyle.Flex; // Show replay button
        _overlayPanel.style.display = DisplayStyle.Flex; // Show Overlay Panel
    }

    // Invoked by event when the user clears the level 
    public void NextLevel()
    {
        Time.timeScale = 0f; // Pause the game 
        levelClearedText.RefreshString(); // Show correct localized label
        nextLevelText.RefreshString(); // Update button text
        _playButton.style.display = DisplayStyle.Flex; // Show Next Level button
        _replayButton.style.display = DisplayStyle.None; // Hide replay button
        
        // Clear existing click handlers and set new one
        _playButton.clicked -= MainMenuGame;
        _playButton.clicked -= TogglePause;
        _playButton.clicked += MainMenuGame; // TO CHANGE IN NEXT LEVEL
        
        _overlayPanel.style.display = DisplayStyle.Flex; // Show Overlay Panel
    }

    private void UpdateLabelText(string translatedText)
    {
        _label.text = translatedText;
    }

    private void UpdatePlayButtonText(string translatedText)
    {
        _playButton.text = translatedText;
    }

    private void OnDestroy()
    {
        // Clean up localization subscriptions
        pausedText.StringChanged -= UpdateLabelText;
        gameOverText.StringChanged -= UpdateLabelText;
        levelClearedText.StringChanged -= UpdateLabelText;
        resumeText.StringChanged -= UpdatePlayButtonText;
        nextLevelText.StringChanged -= UpdatePlayButtonText;
    }
}