using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField] private AudioClip clip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Range(-3f, 3f)]
    [SerializeField] private float pitch = 1f;
    [SerializeField] private float minDelay = 0.1f;

    private AudioSource source;

    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Pitch => pitch;
    public float MinDelay => minDelay;
    private float lastPlayedTime;

    public void Initialize(GameObject hostObject)
    {
        source = hostObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        lastPlayedTime = -minDelay;
    }

    public void Play()
    {
        if (Time.unscaledTime - lastPlayedTime < minDelay)
        {
            return; 
        }

        lastPlayedTime = Time.unscaledTime;
        source.Play();
    }
}