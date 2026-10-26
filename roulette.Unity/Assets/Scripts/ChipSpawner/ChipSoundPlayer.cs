using UnityEngine;

public class ChipSoundPlayer : MonoBehaviour
{
    [SerializeField] private ChipSoundSettings soundSettings = new ChipSoundSettings();
    [SerializeField] private AudioSource audioSourceTemplate; // необязательно: носитель AudioMixer Group

    private AudioSource[] audioPool;
    private int audioPoolIndex;
    private float soundWindowStart;
    private int soundsInWindow;

    public ChipSoundSettings Settings => soundSettings;

    private void Awake()
    {
        BuildAudioPool();
    }

    /// <summary>Вешает звук столкновений на созданную фишку.</summary>
    public void Attach(GameObject chip)
    {
        var sound = chip.GetComponent<ChipCollisionSound>();
        if (sound == null) sound = chip.AddComponent<ChipCollisionSound>();
        sound.Init(this);
    }

    public bool Play(bool chipOnChip, float impact, Vector3 position)
    {
        if (Time.time - soundWindowStart > 0.1f)
        {
            soundWindowStart = Time.time;
            soundsInWindow = 0;
        }
        if (soundsInWindow >= soundSettings.maxSoundsPer100ms) return false;

        AudioClip[] pool = chipOnChip ? soundSettings.chipHitClips : soundSettings.floorHitClips;
        if (pool == null || pool.Length == 0) return false;

        float t = Mathf.InverseLerp(soundSettings.minImpactVelocity, soundSettings.maxImpactVelocity, impact);

        var src = audioPool[audioPoolIndex];
        audioPoolIndex = (audioPoolIndex + 1) % audioPool.Length;

        src.transform.position = position;
        src.clip = pool[Random.Range(0, pool.Length)];
        src.volume = Mathf.Lerp(soundSettings.minVolume, soundSettings.maxVolume, t);
        src.pitch = Random.Range(soundSettings.pitchRange.x, soundSettings.pitchRange.y);
        src.Play();

        soundsInWindow++;
        return true;
    }

    private void BuildAudioPool()
    {
        int count = Mathf.Max(1, soundSettings.audioSourcesCount);
        audioPool = new AudioSource[count];

        var root = new GameObject("ChipAudioPool").transform;
        root.SetParent(transform, false);

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject($"ChipAudio_{i}");
            go.transform.SetParent(root, false);

            var src = go.AddComponent<AudioSource>();
            if (audioSourceTemplate != null)
                src.outputAudioMixerGroup = audioSourceTemplate.outputAudioMixerGroup;

            src.playOnAwake = false;
            src.spatialBlend = 0f;
            audioPool[i] = src;
        }
    }
}
