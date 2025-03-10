using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StateMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private EnduranceManager enduranceManager;

    // UI elements
    private Button _pauseButton;
    private VisualElement _pausePanel;
    private VisualElement _gameOverPanel;
    private VisualElement _nextLevelPanel;
    private Button _resumeButton;
    private Button _quitButton;
    private Button _pauseReplayButton;
    private Button _gameOverReplayButton;
    private Button _nextLevelReplayButton;
    private bool _isPaused; 

    private void Awake()
    {
        // Get base UI elements
        _pauseButton = uiDocument.rootVisualElement.Q<Button>("Pause");
        _pausePanel = uiDocument.rootVisualElement.Q<VisualElement>("PausePanel");
        _gameOverPanel = uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _nextLevelPanel = uiDocument.rootVisualElement.Q<VisualElement>("NextLevelPanel");
        _resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        _quitButton = uiDocument.rootVisualElement.Q<Button>("QuitButton");
        _pauseReplayButton = _pausePanel.Q<Button>("ReplayButton");
        _gameOverReplayButton = _gameOverPanel.Q<Button>("GameOverReplayButton");
        _nextLevelReplayButton = _nextLevelPanel.Q<Button>("NextLevelReplayButton");

        // Add event listeners
        _pauseButton.clicked += TogglePause;
        _resumeButton.clicked += ResumeGame;
        _quitButton.clicked += QuitGame;
        _pauseReplayButton.clicked += ReplayGame;
        _gameOverReplayButton.clicked += ReplayGame;
        _nextLevelReplayButton.clicked += ReplayGame;
    }
    
    private void Update()
    {
        if (!enduranceManager) return;

        // Show Game Over panel when endurance is maxed
        if (enduranceManager.Endurance == EnduranceManager.MaxEndurance)
        {
            _gameOverPanel.style.display = DisplayStyle.Flex;
        }
        // Show Next Level panel when paused with max endurance
        else if (Time.timeScale == 0 && enduranceManager.Endurance != EnduranceManager.MaxEndurance && !_isPaused)
        {
            _nextLevelPanel.style.display = DisplayStyle.Flex;
        }
    }

    private void TogglePause()
    {
        _isPaused = Time.timeScale == 0f;
        Time.timeScale = _isPaused ? 1f : 0f;
        _pausePanel.style.display = _isPaused ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void ResumeGame() => TogglePause();

    private void QuitGame() => SceneManager.LoadScene("MainMenu");

    private void ReplayGame()
    {
        Time.timeScale = 1f; // Ensure time is unpaused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}