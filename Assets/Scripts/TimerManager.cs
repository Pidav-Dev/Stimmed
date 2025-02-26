using UnityEngine;
using UnityEngine.UIElements;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float duration = 60f; // Duration of the level
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    private float _timeRemaining; // Time remaining for the user to clear the level 
    private Label _timerLabel; // The UI Label that displays the timer in the UXML file

    private void Awake()
    {
        _timeRemaining = duration; // Initialize the timer with level's duration 
        _timerLabel = uiDocument.rootVisualElement.Q<Label>("Timer"); // Find the timer label by its name ("Timer")
        // Gets the time remaining and attach to the UI label
        if (_timerLabel != null)
        {
            _timerLabel.text = Mathf.CeilToInt(_timeRemaining).ToString();
        }
    }

    private void Update()
    {
        // Do not update timer when it expires 
        if (_timeRemaining > 0)
        {
            // Subtract the elapsed time from the remaining time
            _timeRemaining -= Time.deltaTime; // It would be 1 * deltaTime
            
            // When the timer reaches zero or below, clamp it and pause the game
            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                Time.timeScale = 0;  // Pause the game
            }

            // Update the UI label to show the current time remaining
            if (_timerLabel != null)
                _timerLabel.text = Mathf.CeilToInt(_timeRemaining).ToString();
        }
    }
}