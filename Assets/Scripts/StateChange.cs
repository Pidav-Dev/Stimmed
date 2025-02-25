using UnityEngine;
using System.Collections;

public class RandomActivation : MonoBehaviour
{
    private bool isActive = false;
    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); // Get the object's Renderer
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color; // Store the original color
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