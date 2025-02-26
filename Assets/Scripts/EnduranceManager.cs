using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class EnduranceManager : MonoBehaviour
{
    public static EnduranceManager Instance;
    public int endurance = 0; // Start at 0 (integer)
    private const int MaxEndurance = 100;
    private const int IncreaseRate = 2; // Integer increase per second
    private const float IncreaseInterval = 1f;

    [SerializeField] private UIDocument uiDocument;
    private Label enduranceLabel;

    private void Awake()
    {
        enduranceLabel = uiDocument.rootVisualElement.Q<Label>("Endurance");
        
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(IncreaseEndurance());
    }

    private void Update()
    {
        enduranceLabel.text = endurance.ToString();
    }

    private IEnumerator IncreaseEndurance()
    {
        while (endurance < MaxEndurance)
        {
            yield return new WaitForSeconds(IncreaseInterval);
            endurance = Mathf.Clamp(endurance + IncreaseRate, 0, MaxEndurance);
            Debug.Log($"Endurance: {endurance}");
        }
    }

    public void ReduceEnduranceByAmount(int amount)
    {
        endurance = Mathf.Max(endurance - amount, 0);
        Debug.Log($"Endurance Reduced! Current: {endurance}");
    }
}