using UnityEngine;

public class BusMovement : MonoBehaviour
{
    [Header("Vibration Settings")]
    public float vibrationIntensity = 0.1f;
    public float vibrationFrequency = 1f;
    
    [Header("Bump Settings")]
    public float bumpIntervalMin = 3f;
    public float bumpIntervalMax = 7f;
    public float bumpForceVertical = 0.5f;
    public float bumpForceHorizontal = 0.3f;
    public float bumpDuration = 0.5f;

    private Vector3 currentBasePosition;
    private Quaternion currentBaseRotation;
    private Vector3 shakeOffset;
    private Vector3 rotationOffset;
    private float nextBumpTime;
    private bool isBumping;

    void Start()
    {
        currentBasePosition = transform.localPosition;
        currentBaseRotation = transform.localRotation;
        nextBumpTime = Time.time + Random.Range(bumpIntervalMin, bumpIntervalMax);
    }

    void Update()
    {
        // Aggiorna la posizione base con eventuali modifiche esterne
        UpdateBaseTransform();

        if (!isBumping)
        {
            // Calcola vibrazioni
            float xShake = Mathf.PerlinNoise(Time.time * vibrationFrequency, 0) * vibrationIntensity;
            float yShake = Mathf.PerlinNoise(0, Time.time * vibrationFrequency) * vibrationIntensity;
            float zShake = Mathf.PerlinNoise(Time.time * vibrationFrequency, Time.time * vibrationFrequency) * vibrationIntensity;
            
            shakeOffset = new Vector3(xShake, yShake, zShake) * 0.5f;
        }

        // Controlla scossoni
        if (Time.time > nextBumpTime && !isBumping)
        {
            StartCoroutine(ApplyBump());
            nextBumpTime = Time.time + Random.Range(bumpIntervalMin, bumpIntervalMax);
        }
    }

    void LateUpdate()
    {
        // Applica offset mantenendo la posizione base
        transform.localPosition = currentBasePosition + shakeOffset;
        transform.localRotation = currentBaseRotation * Quaternion.Euler(rotationOffset);
    }

    void UpdateBaseTransform()
    {
        // Mantiene la posizione base aggiornata con le modifiche esterne
        // (rimuove l'offset precedente per ottenere la posizione "pulita")
        currentBasePosition = transform.localPosition - shakeOffset;
        currentBaseRotation = transform.localRotation * Quaternion.Inverse(Quaternion.Euler(rotationOffset));
    }

    System.Collections.IEnumerator ApplyBump()
    {
        isBumping = true;
        float elapsed = 0f;
        Vector3 bumpOffset = new Vector3(
            Random.Range(-bumpForceHorizontal, bumpForceHorizontal),
            bumpForceVertical,
            Random.Range(-bumpForceHorizontal, bumpForceHorizontal)
        );

        Vector3 bumpRotation = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        Vector3 initialShake = shakeOffset;
        Vector3 targetShake = shakeOffset + bumpOffset;

        Vector3 initialRotation = rotationOffset;
        Vector3 targetRotation = rotationOffset + bumpRotation;

        while (elapsed < bumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bumpDuration;

            shakeOffset = Vector3.Lerp(initialShake, targetShake, t);
            rotationOffset = Vector3.Lerp(initialRotation, targetRotation, t);
            
            yield return null;
        }

        // Ritorno graduale alla vibrazione normale
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, elapsed / 0.3f);
            rotationOffset = Vector3.Lerp(rotationOffset, Vector3.zero, elapsed / 0.3f);
            yield return null;
        }

        isBumping = false;
    }
}