using AYellowpaper.SerializedCollections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<string, Sound> sounds;

    void Awake()
    {
        foreach (Sound s in sounds.Values)
        {
            s.Initialize(gameObject);
        }
    }

    public void Play(string name)
    {
        sounds[name].Play();
    }
}
