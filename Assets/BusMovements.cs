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

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private float nextBumpTime;
    private bool isBumping;

    void Start()
    {
        // Salva la posizione e rotazione originali relative al parent
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        
        // Imposta il tempo per il primo scossone
        nextBumpTime = Time.time + Random.Range(bumpIntervalMin, bumpIntervalMax);
    }

    void Update()
    {
        if (!isBumping)
        {
            // Vibrazione base costante usando il Perlin Noise
            float xShake = Mathf.PerlinNoise(Time.time * vibrationFrequency, 0) * vibrationIntensity;
            float yShake = Mathf.PerlinNoise(0, Time.time * vibrationFrequency) * vibrationIntensity;
            float zShake = Mathf.PerlinNoise(Time.time * vibrationFrequency, Time.time * vibrationFrequency) * vibrationIntensity;
            
            transform.localPosition = originalLocalPosition + new Vector3(xShake, yShake, zShake) * 0.5f;
        }

        // Controlla se è il momento per uno scossone più forte
        if (Time.time > nextBumpTime && !isBumping)
        {
            StartCoroutine(ApplyBump());
            nextBumpTime = Time.time + Random.Range(bumpIntervalMin, bumpIntervalMax);
        }
    }

    System.Collections.IEnumerator ApplyBump()
    {
        isBumping = true;
        float elapsed = 0f;
        Vector3 bumpDirection = new Vector3(
            Random.Range(-bumpForceHorizontal, bumpForceHorizontal),
            bumpForceVertical,
            Random.Range(-bumpForceHorizontal, bumpForceHorizontal)
        );

        Quaternion startRot = transform.localRotation;
        Vector3 startPos = transform.localPosition;

        // Aggiungi una rotazione casuale per lo scossone
        Vector3 randomRot = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        while (elapsed < bumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bumpDuration;

            // Applica il movimento
            transform.localPosition = Vector3.Lerp(startPos, originalLocalPosition + bumpDirection, t);
            
            // Applica la rotazione
            transform.localRotation = Quaternion.Lerp(startRot, originalLocalRotation * Quaternion.Euler(randomRot), t);
            
            yield return null;
        }

        // Ritorno graduale alla posizione originale
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, elapsed / 0.3f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalLocalRotation, elapsed / 0.3f);
            yield return null;
        }

        isBumping = false;
    }

    void LateUpdate()
    {
        // Mantieni un ritorno costante verso la posizione originale
        if (!isBumping)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalLocalRotation, Time.deltaTime * 5f);
        }
    }
}