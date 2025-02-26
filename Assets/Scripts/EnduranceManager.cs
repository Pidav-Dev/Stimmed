using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class EnduranceManager : MonoBehaviour
{
    public static EnduranceManager Instance; // Used to reference the endurance in other scripts
    [SerializeField] private int endurance; // Level of endurance that describes character's sensory overload 
    [SerializeField] private UIDocument uiDocument;
    private const int MaxEndurance = 100; // Means character is sensory overloaded and you lost
    private const int IncreaseRate = 2; // Integer increase per second
    private const float IncreaseInterval = 1f; // Number of seconds to wait before increasing endurance
    private Label _enduranceLabel; // Label where it will be attached 

    private void Awake()
    {
        _enduranceLabel = uiDocument.rootVisualElement.Q<Label>("Endurance"); // Find the UI element in UXML file and attach to it
        
        // Instance is null, there are no duplicates 
        if (Instance == null)
            Instance = this;
        // Instance is not null, so there are duplicates 
        else
        {
            Destroy(gameObject); // Destroy actual game object to not manage duplicates
            return; // Skips the iteration if there were duplicates 
        }
        // Preserves the only instance there is
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        endurance = 0; 
        // Fork the process and concurrently executes another routine
        StartCoroutine(IncreaseEndurance());
    }

    private void Update()
    {
        // Update label
        _enduranceLabel.text = endurance.ToString();
    }

    // Executed concurrently in this process, it increments the endurance
    private IEnumerator IncreaseEndurance()
    {
        while (endurance < MaxEndurance)
        {
            // yield return gives control to Unity engine, so it will wait the child process to finish
            yield return new WaitForSeconds(IncreaseInterval); // Before incrementing, it waits for some time 
            endurance = Mathf.Clamp(endurance + IncreaseRate, 0, MaxEndurance); // Increments endurance and clamps it 
            Debug.Log($"Endurance: {endurance}");
        }
    }

    // Reduce the endurance when the user clicks on stimuli
    public void IncreaseEnduranceByAmount(int amount)
    {
        endurance = Mathf.Max(endurance - amount, 0); // Reduces if the following value will be positive, otherwise it doesn't do anything 
        Debug.Log($"Endurance Reduced! Current: {endurance}");
    }

    // Returns endurance following information hiding principles
    public int GetEndurance()
    {
        return endurance;
    }
}