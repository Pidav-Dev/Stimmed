using UnityEngine;
using System.Collections;

public class EnduranceManager : MonoBehaviour
{
    public static EnduranceManager Instance; // Singleton instance
    public float endurance = 100f; // Start from 100
    private float minEndurance = 0f;
    private float enduranceDecreaseRate = 2f; // Speed of endurance decrease per second
    private float decreaseInterval = 1f; // Time interval for decrease (1 second)

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        DontDestroyOnLoad(gameObject); // Keep instance alive
    }

    private void Start()
    {
        StartCoroutine(DecreaseEndurance());
    }

    private IEnumerator DecreaseEndurance()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);
            endurance = Mathf.Clamp(endurance - enduranceDecreaseRate, minEndurance, 100f);
            Debug.Log("Endurance: " + endurance);
        }
    }

    public void IncreaseEndurance(float amount)
    {
        endurance = Mathf.Clamp(endurance + amount, minEndurance, 100f);
        Debug.Log("Endurance Increased! Current Endurance: " + endurance);
    }
}