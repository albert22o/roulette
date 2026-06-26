using UnityEngine;

[RequireComponent(typeof(Light))]
public class SmoothFlicker : MonoBehaviour
{
    private Light targetLight;

    [Header("Настройки")]
    public float baseIntensity = 1f;
    public float flickerAmount = 0.3f;
    public float speed = 10f;

    private float noiseOffset;

    private void Awake()
    {
        targetLight = GetComponent<Light>();

        // У каждого светильника своё случайное смещение
        noiseOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(
            Time.time * speed + noiseOffset,
            noiseOffset
        );

        targetLight.intensity =
            baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}