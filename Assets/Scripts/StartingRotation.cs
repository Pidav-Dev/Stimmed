using UnityEngine;
using UnityEngine.InputSystem;

public class StartingRotation : MonoBehaviour
{
    public Transform pivot; // Assign the pivot point in the Inspector
    public float rotationSpeed = 180f; // Degrees per second
    private bool shouldRotate = false;
    private float remainingAngle = 360f;

    void Start()
    {
        StartRotation();
    }

    void Update()
    {
        if (shouldRotate && Time.timeScale > 0f)
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            transform.position = pivot.position + Quaternion.Euler(0, rotationStep, 0) * (transform.position - pivot.position);
            transform.LookAt(pivot);
            remainingAngle -= rotationStep;

            if (remainingAngle <= 0f)
            {
                shouldRotate = false;
                remainingAngle = 360f; // Reset for next use
                this.enabled = false; // Disable the script after rotation completes
            }
        }
    }

    public void StartRotation()
    {
        if (!shouldRotate)
        {
            Debug.Log("CIAO");
            shouldRotate = true;
            remainingAngle = 315f;
            this.enabled = true; // Enable the script when starting rotation
        }
    }
}
