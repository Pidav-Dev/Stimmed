using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class TimerManager : MonoBehaviour
{
    [Header("Level Timer")]
    [SerializeField] private float duration = 60f; // Duration of the level
    [SerializeField] private UIDocument uiDocument; // UI document to link the timer to a label 
    
    public UnityEvent winningCondition; // Event to manage the winning condition
    
    private EnduranceManager _character; // Component from which get the endurance
    private float _timeRemaining; // Time in seconds remaining for the user to clear the level 
    private Label _timerLabel; // The UI Label that displays the timer in the UXML file

    // State variables for the timer in order to display it as common mm:ss
    private int _minutes;
    private int _seconds; 

    private void Awake()
    {
        _timeRemaining = duration; // Initialize the timer with level's duration 
        _timerLabel = uiDocument.rootVisualElement.Q<Label>("Timer"); // Find the timer label by its name ("Timer")
        _character = GetComponent<EnduranceManager>(); // Assign component from the own GameObject
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
            // Assign time components from generalized timer 
            _minutes = Mathf.Clamp(Mathf.FloorToInt(_timeRemaining / 60), 0, 59);
            _seconds = Mathf.Clamp(Mathf.FloorToInt(_timeRemaining % 60), 0, 59);
            
            // When the timer reaches zero or below, clamp it and pause the game
            if (_timeRemaining <= 0 || _character.Endurance >= 100)
            {
                Time.timeScale = 0;  // Pause the game
            }

            // Update the UI label to show the current time remaining
            if (_timerLabel != null)
            {
                _timerLabel.text =
                    (_minutes >= 10 ? "" : "0") + Mathf.CeilToInt(_minutes) +
                    ":" +
                    (_seconds >= 10 ? "" : "0") + Mathf.CeilToInt(_seconds);
            }
        } 
        // When the timer runs out, the user wins
        else if (_timeRemaining <= 0)
        {
            winningCondition?.Invoke();
        }
    }
}