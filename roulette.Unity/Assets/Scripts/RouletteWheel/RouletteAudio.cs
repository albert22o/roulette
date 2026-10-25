using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Весь звук рулетки. Знает только про RouletteWheel (его события и InnerWheel).
/// Рассчитан на несколько одновременно катящихся шаров.
/// </summary>
public class RouletteAudio : MonoBehaviour
{

    [Header("а) Качение шаров (один общий луп)")]
    [SerializeField] private IntensityLoop ballRoll;

    [Header("б) Стуки о препятствия")]
    [SerializeField] private AudioSource hitSourceTemplate;  // настройки (mixer, 3D и т.д.) копируются в пул
    [SerializeField, Min(1)] private int hitVoices = 8;      // сколько ударов могут звучать одновременно
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AnimationCurve hitVolumeByImpact = AnimationCurve.Linear(0, 0.1f, 1, 1);
    [SerializeField] private Vector2 hitPitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private float minHitIntervalPerBall = 0.05f; // троттлинг на каждый шар отдельно
    [SerializeField] private float minHitImpact = 0.05f;

    [Header("в) Кручение рулетки")]
    [SerializeField] private IntensityLoop wheelSpin;
    [SerializeField] private float maxWheelSpeed = 360f;     // град/с, которым соответствует 1

    private readonly Dictionary<int, float> rolling = new Dictionary<int, float>();
    private readonly Dictionary<int, float> lastHitByBall = new Dictionary<int, float>();
    private AudioSource[] hitVoicePool;
    private int nextVoice;
    private Quaternion lastWheelRotation;

    private RouletteWheel wheel;

    [Inject]
    public void Construct(RouletteWheel wheel)
    {
        this.wheel = wheel;
        BuildHitPool();
    }

    private void OnEnable()
    {
        wheel.BallRollIntensityChanged += OnBallRoll;
        wheel.BallCollided += OnBallCollided;
        lastWheelRotation = wheel.InnerWheel.localRotation;
    }

    private void OnDisable()
    {
        wheel.BallRollIntensityChanged -= OnBallRoll;
        wheel.BallCollided -= OnBallCollided;
        rolling.Clear();
        lastHitByBall.Clear();
        ballRoll.SetTarget(0f);
    }

    // ---------- Качение ----------

    private void OnBallRoll(int ballId, float intensity)
    {
        if (intensity <= 0f) rolling.Remove(ballId);
        else rolling[ballId] = intensity;

        // Громкость: «мягкая сумма», 1 - Π(1 - v). Несколько шаров громче одного, но без клиппинга.
        // Высота: среднее, взвешенное по интенсивности (быстрые шары тянут тон вверх).
        float inverse = 1f, sum = 0f, sumSq = 0f;
        foreach (var v in rolling.Values)
        {
            inverse *= 1f - v;
            sum += v;
            sumSq += v * v;
        }

        float volumeIntensity = 1f - inverse;
        float pitchIntensity = sum > 0f ? sumSq / sum : 0f;
        ballRoll.SetTarget(volumeIntensity, pitchIntensity);
    }

    // ---------- Удары ----------

    private void BuildHitPool()
    {
        if (hitSourceTemplate == null)
        {
            Debug.LogError($"{nameof(RouletteAudio)}: hitSourceTemplate не назначен", this);
            return;
        }

        hitVoicePool = new AudioSource[hitVoices];
        for (int i = 0; i < hitVoices; i++)
        {
            var src = i == 0 ? hitSourceTemplate : CloneSource(hitSourceTemplate);
            src.playOnAwake = false;
            src.loop = false;
            hitVoicePool[i] = src;
        }
    }

    private static AudioSource CloneSource(AudioSource template)
    {
        var go = new GameObject(template.name + "_voice");
        go.transform.SetParent(template.transform.parent, false);
        go.transform.localPosition = template.transform.localPosition;

        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = template.outputAudioMixerGroup;
        src.spatialBlend = template.spatialBlend;
        src.minDistance = template.minDistance;
        src.maxDistance = template.maxDistance;
        src.rolloffMode = template.rolloffMode;
        src.dopplerLevel = template.dopplerLevel;
        src.spread = template.spread;
        src.panStereo = template.panStereo;
        src.priority = template.priority;
        return src;
    }

    private void OnBallCollided(int ballId, float impact)
    {
        if (hitVoicePool == null || hitClips == null || hitClips.Length == 0) return;
        if (impact < minHitImpact) return;

        float now = Time.time;
        if (lastHitByBall.TryGetValue(ballId, out float last) && now - last < minHitIntervalPerBall) return;

        if (lastHitByBall.Count > 64) lastHitByBall.Clear(); // шары приходят и уходят, не копим мусор
        lastHitByBall[ballId] = now;

        // Round-robin: каждый удар на своём голосе, поэтому pitch у каждого свой.
        // Если все голоса заняты, перебивается самый старый.
        var voice = hitVoicePool[nextVoice];
        nextVoice = (nextVoice + 1) % hitVoicePool.Length;

        voice.clip = hitClips[Random.Range(0, hitClips.Length)];
        voice.pitch = Random.Range(hitPitchRange.x, hitPitchRange.y);
        voice.volume = hitVolumeByImpact.Evaluate(impact);
        voice.Play();
    }

    // ---------- Кручение ----------

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt > 0f)
        {
            var rot = wheel.InnerWheel.localRotation;
            float angularSpeed = Quaternion.Angle(lastWheelRotation, rot) / dt;
            lastWheelRotation = rot;
            wheelSpin.SetTarget(angularSpeed / maxWheelSpeed);
        }

        ballRoll.Tick(dt);
        wheelSpin.Tick(dt);
    }
}