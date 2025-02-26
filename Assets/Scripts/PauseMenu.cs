using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; // Assign this in the Inspector!

    private Button pauseButton;
    private VisualElement pausePanel;
    private Button resumeButton;
    private Button quitButton;

    private void Awake()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned!");
            return;
        }

        // Get root element
        var root = uiDocument.rootVisualElement;

        // Find pause button (name="Pause")
        pauseButton = root.Q<Button>("Pause");
        if (pauseButton == null) Debug.LogError("Pause button not found!");

        // Find pause panel (name="PausePanel")
        pausePanel = root.Q<VisualElement>("PausePanel");
        if (pausePanel == null) Debug.LogError("Pause panel not found!");

        // Find resume and quit buttons
        resumeButton = root.Q<Button>("ResumeButton");
        quitButton = root.Q<Button>("QuitButton");

        // Add event listeners
        pauseButton.clicked += TogglePause;
        resumeButton.clicked += ResumeGame;
        quitButton.clicked += QuitGame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        pausePanel.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void ResumeGame()
    {
        TogglePause();
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}