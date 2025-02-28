using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StateMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // Assign this in the Inspector!
    // UI elements
    private Button _pauseButton;
    private VisualElement _pausePanel;
    private VisualElement _gameOverPanel;
    private Button _resumeButton;
    private Button _quitButton;
    private Button _replayButton;
    
    [SerializeField] private EnduranceManager _enduranceManager;
    

    private void Awake()
    {
        // Find UI elements (should add if (element == null) Debug.LogError("Element not found!"); but bored)
        _pauseButton = uiDocument.rootVisualElement.Q<Button>("Pause");
        _pausePanel = uiDocument.rootVisualElement.Q<VisualElement>("PausePanel");
        _gameOverPanel = uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        _quitButton = uiDocument.rootVisualElement.Q<Button>("QuitButton");
        _replayButton = uiDocument.rootVisualElement.Q<Button>("ReplayButton");

        // Add event listeners
        _pauseButton.clicked += TogglePause;
        _resumeButton.clicked += ResumeGame;
        _quitButton.clicked += QuitGame;
        _replayButton.clicked += ReplayGame;
    }

    private void Update()
    {
        // Checks if escape button is pressed and then toggles pause (for debugging only) 
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
        if (_enduranceManager && _enduranceManager.GetEndurance() == 100)
        {
            _gameOverPanel.style.display = DisplayStyle.Flex;
        }
    }

    // Called when the pause button is pressed 
    private void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f; // Checks if the application is running 
        Time.timeScale = isPaused ? 1f : 0f; // Resume based on the previous checking 
        _pausePanel.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex; // Displays the panel based on pause
    }

    // Called when the resume button is pressed 
    private void ResumeGame()
    {
        TogglePause();
    }

    // Called when the quit button is pressed
    private void QuitGame()
    {
        // Preprocessor directive that checks if the application is running in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Uses Unity Editor's way of closing the app
        #else
        Application.Quit(); // Uses the particular build's way of closing the app if it is not running in the Unity Editor
        #endif
    }

    private void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}