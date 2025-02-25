using UnityEngine;
using UnityEngine.UI; // If using standard UI Text
// using TMPro; // Uncomment if using TextMeshPro

public class EnduranceUI : MonoBehaviour
{
    public Text enduranceText; // Reference to the UI Text (for standard UI)
    // public TMP_Text enduranceText; // Uncomment if using TextMeshPro
    private float updateInterval = 0.1f; // Update frequency for endurance stat display

    void Start()
    {
        if (enduranceText == null)
        {
            Debug.LogError("Endurance Text is not assigned!");
        }
    }

    void Update()
    {
        if (EnduranceManager.Instance != null)
        {
            // Update the text with the current endurance value
            enduranceText.text = "Endurance: " + EnduranceManager.Instance.endurance.ToString("F1");
        }
    }
}