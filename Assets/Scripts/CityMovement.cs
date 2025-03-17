using UnityEngine;

public class PacManWrap : MonoBehaviour
{
    [Header("Repeating area limit")]
    public float zMin = -10f; // Spawning offset
    public float zMax = 10f; // World limit
    public float velocity = 10f; // Bus fake velocity

    void Update()
    {
        Vector3 newPosition = transform.position + velocity * Vector3.forward * Time.deltaTime; // Bus fake movement

        // Limit control over Z axis
        if (newPosition.z > zMax)
            newPosition.z = zMin;
        else if (newPosition.z < zMin)
            newPosition.z = zMax;

        transform.position = newPosition; // Position reassigning
    }
}