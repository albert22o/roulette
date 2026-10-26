using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private bool started;

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true;
    }

    public void Play()
    {
        if (audioSource == null) return;

        if (started)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Play();
            started = true;
        }
    }

    public void Pause()
    {
        if (audioSource != null)
            audioSource.Pause();
    }
}