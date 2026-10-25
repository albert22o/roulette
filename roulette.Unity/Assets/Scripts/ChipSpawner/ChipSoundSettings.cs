using System;
using UnityEngine;

[Serializable]
public class ChipSoundSettings
{
    [Header("Clips")]
    public AudioClip[] chipHitClips;   
    public AudioClip[] floorHitClips;  

    [Header("Floor")]
    [Tooltip("Слои, которые считаются полом")]
    public LayerMask floorMask = ~0;

    [Header("Volume / Pitch")]
    public float minImpactVelocity = 0.5f;
    public float maxImpactVelocity = 6f;
    [Range(0f, 1f)] public float minVolume = 0.1f;
    [Range(0f, 1f)] public float maxVolume = 0.8f;
    public Vector2 pitchRange = new Vector2(0.9f, 1.15f);

    [Header("Anti-spam")]
    [Tooltip("Интервал между звуками одной фишки")]
    public float perChipCooldown = 0.06f;
    [Tooltip("Столкновения сразу после спауна игнорируются")]
    public float spawnGraceTime = 0.15f;
    [Tooltip("Размер пула AudioSource (макс. одновременных звуков)")]
    public int audioSourcesCount = 12;
    [Tooltip("Макс. звуков за окно 0.1 с")]
    public int maxSoundsPer100ms = 8;
}