using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class WinningSequenceSound : MonoBehaviour
{
    [Serializable]
    public class ColorClip
    {
        public ColorType color;
        public AudioClip clip;
    }

    [Serializable]
    public class DozenClip
    {
        public DozenType dozen;
        public AudioClip clip;
    }

    [Header("Подсветка секторов")]
    [Tooltip("Клип для конкретного цвета (этап 1)")]
    [SerializeField] private ColorClip[] colorClips;
    [Tooltip("Клип для конкретной дюжины (этап 2)")]
    [SerializeField] private DozenClip[] dozenClips;
    [Tooltip("Клип для выигравшего номера (этап 3)")]
    [SerializeField] private AudioClip numberClip;
    [Tooltip("Клип для нуля (этап 3). Если пусто, играет Number Clip")]
    [SerializeField] private AudioClip zeroClip;
    [Tooltip("Запасной клип, если для сектора ничего не задано")]
    [SerializeField] private AudioClip defaultStageClip;
    [SerializeField, Range(0f, 1f)] private float stageVolume = 0.7f;
    [SerializeField] private float stageBasePitch = 1f;
    [Tooltip("Прибавка к pitch на каждом следующем этапе")]
    [SerializeField] private float stagePitchStep = 0.1f;

    [Header("Рост множителя")]
    [SerializeField] private AudioClip multiplierUpClip;
    [SerializeField, Range(0f, 1f)] private float multiplierVolume = 0.8f;
    [SerializeField] private float multiplierBasePitch = 1f;
    [Tooltip("Прибавка к pitch на каждое повышение множителя за раунд")]
    [SerializeField] private float multiplierPitchStep = 0.15f;
    [SerializeField] private float multiplierMaxPitch = 2f;

    [Header("Итоговое умножение денег")]
    [Tooltip("Играет в момент начала фазы CountMoney")]
    [SerializeField] private AudioClip countMoneyClip;
    [Tooltip("Играет при завершении выигрышного раунда (деньги начислены)")]
    [SerializeField] private AudioClip moneyReceivedClip;
    [SerializeField, Range(0f, 1f)] private float moneyVolume = 0.9f;

    private WinningSequence winningSequence;
    [SerializeField] private AudioSource stageSource;
    [SerializeField] private AudioSource multiplierSource;
    [SerializeField] private AudioSource moneySource;

    private HitInfo currentHit;
    private bool hasHit;
    private float lastMultiplier;
    private int multiplierIncreases;
    private bool subscribed;

    [Inject]
    public void Construct(WinningSequence winningSequence)
    {
        this.winningSequence = winningSequence;
    }

    private void Start()
    {
        winningSequence.SequenceStart += OnSequenceStart;
        winningSequence.Highlight += OnHighlightStarted;
        winningSequence.DisplayMultiplier += OnMultiplierChanged;
        winningSequence.MoneyCountStarted += OnCountMoney;
        winningSequence.EndSequence += OnEndSequence;
        winningSequence.StageMatched += OnStageMatched;
        subscribed = true;
    }

    private void OnDestroy()
    {
        if (!subscribed) return;

        winningSequence.SequenceStart -= OnSequenceStart;
        winningSequence.Highlight -= OnHighlightStarted;
        winningSequence.DisplayMultiplier -= OnMultiplierChanged;
        winningSequence.MoneyCountStarted -= OnCountMoney;
        winningSequence.EndSequence -= OnEndSequence;
        winningSequence.StageMatched -= OnStageMatched;
    }

    private void OnStageMatched(BetButton.HighlightPhase phase)
    {
        if (!hasHit) return;
        PlayStage(phase, currentHit);
    }

    private void ResetState()
    {
        lastMultiplier = 0f;
        multiplierIncreases = 0;
        hasHit = false;
    }

    private void OnSequenceStart(int betSum, int unused, float rerollCoeff) => ResetState();

    private void OnHighlightStarted(BetInfo betInfo, HitInfo hitInfo)
    {
        currentHit = hitInfo;
        hasHit = true;
    }

    private void OnMultiplierChanged(float multiplier)
    {
        if (multiplier <= lastMultiplier) return;

        lastMultiplier = multiplier;
        multiplierIncreases++;

        if (multiplierUpClip == null) return;

        multiplierSource.pitch = Mathf.Min(
            multiplierMaxPitch,
            multiplierBasePitch + multiplierPitchStep * (multiplierIncreases - 1));
        multiplierSource.PlayOneShot(multiplierUpClip, multiplierVolume);
    }

    private void PlayStage(BetButton.HighlightPhase phase, HitInfo hitInfo)
    {
        AudioClip clip = ResolveStageClip(phase, hitInfo);
        if (clip == null) return;

        stageSource.pitch = stageBasePitch + stagePitchStep * (int)phase;
        stageSource.PlayOneShot(clip, stageVolume);
    }

    private AudioClip ResolveStageClip(BetButton.HighlightPhase phase, HitInfo hitInfo)
    {
        AudioClip clip = null;

        switch (phase)
        {
            case BetButton.HighlightPhase.Color:
                if (colorClips != null)
                {
                    foreach (var c in colorClips)
                    {
                        if (c.color == hitInfo.FieldColor && c.clip != null)
                        {
                            clip = c.clip;
                            break;
                        }
                    }
                }
                break;

            case BetButton.HighlightPhase.Dozen:
                if (dozenClips != null)
                {
                    foreach (var d in dozenClips)
                    {
                        if (d.dozen == hitInfo.Dozen && d.clip != null)
                        {
                            clip = d.clip;
                            break;
                        }
                    }
                }
                break;

            case BetButton.HighlightPhase.Number:
                clip = (hitInfo.FieldNumber == 0 && zeroClip != null) ? zeroClip : numberClip;
                break;
        }

        return clip != null ? clip : defaultStageClip;
    }

    private void OnCountMoney(float duration)
    {
        if (countMoneyClip == null) return;

        if (duration == 0) return;
        float pitch = duration > 0f ? countMoneyClip.length / duration : 1f;
        moneySource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
        moneySource.PlayOneShot(countMoneyClip, moneyVolume);
    } 

    private void OnEndSequence(EndSequenceType endSequenceType)
    {
        var isRoundWon = (endSequenceType == EndSequenceType.WinGame) || (endSequenceType == EndSequenceType.WinRound);

        if (isRoundWon && moneyReceivedClip != null)
        {
            moneySource.pitch = 1f;
            moneySource.PlayOneShot(moneyReceivedClip, moneyVolume);
        }

        ResetState();
    }
}