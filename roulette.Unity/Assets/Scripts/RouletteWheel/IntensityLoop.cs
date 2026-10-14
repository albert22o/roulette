using System;
using UnityEngine;

/// <summary>
/// Зацикленный звук, громкость и высота которого зависят от интенсивности 0..1.
/// Громкость и высота могут задаваться раздельно (нужно, когда источников несколько).
/// </summary>
[Serializable]
public class IntensityLoop
{
    [SerializeField] private AudioSource source;   // loop = true, playOnAwake = false
    [SerializeField] private AnimationCurve volumeByIntensity = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 pitchRange = new Vector2(0.8f, 1.3f);
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float pitchSmoothing = 10f;

    private float volumeTarget, volumeCurrent;
    private float pitchTarget, pitchCurrent;

    public void SetTarget(float intensity) => SetTarget(intensity, intensity);

    public void SetTarget(float volumeIntensity, float pitchIntensity)
    {
        volumeTarget = Mathf.Clamp01(volumeIntensity);

        // Пока звук затухает до нуля, высоту не трогаем, иначе слышен «слайд» вниз
        if (volumeTarget > 0.001f)
            pitchTarget = Mathf.Clamp01(pitchIntensity);
    }

    public void Tick(float deltaTime)
    {
        if (source == null) return;

        volumeCurrent = Mathf.MoveTowards(volumeCurrent, volumeTarget, fadeSpeed * deltaTime);
        pitchCurrent = Mathf.Lerp(pitchCurrent, pitchTarget, 1f - Mathf.Exp(-pitchSmoothing * deltaTime));

        source.volume = volumeByIntensity.Evaluate(volumeCurrent) * maxVolume;
        source.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, pitchCurrent);

        bool shouldPlay = volumeCurrent > 0.001f;
        if (shouldPlay && !source.isPlaying) source.Play();
        else if (!shouldPlay && source.isPlaying) source.Pause();
    }
}