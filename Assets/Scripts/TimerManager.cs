using UnityEngine;
using UnityEngine.UIElements;

public class TimerManager : MonoBehaviour
{
    // Total time for the timer (in seconds)
    public float startTime = 60f;
    private float timeRemaining;

    // Reference to the UIDocument that contains UI elements
    [SerializeField] private UIDocument uiDocument;
    
    // The UI Label that displays the timer in the UXML file
    private Label timerLabel;

    private void Awake()
    {
        // Initialize the timer
        timeRemaining = startTime;
        // Find the timer label by its name ("Timer")
        timerLabel = uiDocument.rootVisualElement.Q<Label>("Timer");
        if (timerLabel != null)
            timerLabel.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            // Subtract the elapsed time from the remaining time
            timeRemaining -= Time.deltaTime;
            
            // When the timer reaches zero or below, clamp it and pause the game
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                Time.timeScale = 0;  // Pause the game
            }

            // Update the UI label to show the current time remaining
            if (timerLabel != null)
                timerLabel.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }
}