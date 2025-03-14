using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StateMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
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
    }

    // Called when the user taps on pause button or resume button after pause
    private void TogglePause()
    {
        // Pause or resume the game based on actual state
        var isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        
        _label.text = "Paused"; // Show correct label
        
        // Show the correct button in the Overlay Panel to resume the game
        _playButton.text = "Resume";
        _playButton.clicked += TogglePause;
        
        // Show the Overlay Panel based on actual state
        _overlayPanel.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
    }
    
    // Called when user taps on replay button in Overlay Panel
    private void ReplayGame()
    {
        Time.timeScale = 1f; // Ensure time is unpaused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single); // Reload actual scene
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
        _label.text = "Game Over"; // Show correct label
        _playButton.style.display = DisplayStyle.None; // Remove Resume button
        _overlayPanel.style.display = DisplayStyle.Flex; // Show Overlay Panel
    }

    // Invoked by event when the user clears the level 
    public void NextLevel()
    {
        Time.timeScale = 0f; // Pause the game 
        _label.text = "Level cleared"; // Show correct label
        _playButton.text = "NextLevel"; // Show the correct button in the Overlay Panel to play next level
        _playButton.clicked += MainMenuGame; // TO CHANGE IN NEXT LEVEL
        _overlayPanel.style.display = DisplayStyle.Flex; // Show Overlay Panel
    }
}