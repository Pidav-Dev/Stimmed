using UnityEngine;
using System.Collections;

public class StateChange : MonoBehaviour
{
    private bool isActive = false;
    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        StartCoroutine(ActivateRandomly());
    }

    IEnumerator ActivateRandomly()
    {
        while (true)
        {
            float waitTime = Random.Range(0f, 10f);
            yield return new WaitForSeconds(waitTime);
            if (!isActive) 
            {
                isActive = true;
                ChangeColor(Color.red);
                Debug.Log(gameObject.name + " Activated (Color: Red)");
            }
        }
    }

    void OnMouseDown()
    {
        if (isActive)
        {
            isActive = false;
            ChangeColor(originalColor);
            // Reduce universal endurance when clicking any active object
            if (EnduranceManager.Instance != null)
            {
                Debug.Log("Before Reduction: " + EnduranceManager.Instance.endurance);
                EnduranceManager.Instance.ReduceEnduranceByAmount(10); // Integer reduction
                Debug.Log("After Reduction: " + EnduranceManager.Instance.endurance);
            }
            else
            {
                Debug.LogError("EnduranceManager instance is missing from the scene!");
            }
            Debug.Log(gameObject.name + " Deactivated (Color: Original)");
        }
    }

    void ChangeColor(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
}