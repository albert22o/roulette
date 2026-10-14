using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

[RequireComponent(typeof(Animator))]
public class RouletteBall : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private OutlineController outline;
    [SerializeField] private BallVisual visual;
    [SerializeField] private VisualEffect vfx;

    [Header("Roll intensity")]
    [SerializeField] private float maxRollSpeed = 10f;       // скорость (в единицах физ. модели), которой соответствует 1
    [SerializeField] private float intensitySmoothing = 15f;
    [SerializeField] private float intensityEpsilon = 0.01f; // не шлём событие при мелких изменениях

    [Header("Collision")]
    [SerializeField] private float maxImpactSpeed = 8f;      // сила удара, которой соответствует 1

    private Transform rouletteOrigin;
    private Ball ball;
    private static int nextId;
    private float scaleMultiplier;
    private bool isInteractionAvailable;

    private bool updateFrozen = false;
    private bool isMouseHover;

    private Animator animator;

    private Vector3 lastModelPosition;
    private bool hasLastPosition;
    private float currentIntensity;
    private bool isRolling = true; // шарик катится, пока не остановился / не анимируется

    public event Action<string> Hover;
    public event Action<string> UnHover;
    public event Action<string> Clicked;
    /// <summary>Уникальный id экземпляра (hash может повторяться при респавне).</summary>
    public int Id { get; private set; }

    public event Action<int, float> RollIntensityChanged; // ballId, 0..1
    public event Action<int, float> Collided;             // ballId, 0..1
    public event Action Destroyed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Id = ++nextId;
    }

    public void Initialize(Ball ball, float scaleMultiplier, Transform rouletteOrigin)
    {
        this.ball = ball;
        this.scaleMultiplier = scaleMultiplier;
        this.rouletteOrigin = rouletteOrigin;

        ball.Destroyed += OnBallDestroy;
        ball.Animated += OnBallAnimated;
        ball.Stopped += OnBallStop;
        ball.Collided += OnBallCollided;
    }

    public void SetVisualComponent(VisualComponent visualComponent) => visual.SetVisualComponent(visualComponent);

    private void OnBallStop()
    {
        isRolling = false;
        SetIntensity(0f);

        isInteractionAvailable = true;
        if (isMouseHover)
        {
            Hover?.Invoke(ball.Hash);
            outline.ToggleMaterial(true);
        }
    }

    private void OnBallCollided(float impactSpeed)
    {
        Collided?.Invoke(Id, Mathf.Clamp01(impactSpeed / maxImpactSpeed));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHover = true;
        if (isInteractionAvailable == false) return;
        Hover?.Invoke(ball.Hash);
        outline.ToggleMaterial(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHover = false;
        if (isInteractionAvailable == false) return;
        UnHover?.Invoke(ball.Hash);
        outline.ToggleMaterial(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInteractionAvailable == false) return;
        Clicked?.Invoke(ball.Hash);
    }

    public void OnBallAnimated()
    {
        isRolling = false;
        SetIntensity(0f);

        FreezeUpdate();
        transform.rotation = Quaternion.Euler(Vector3.up);
        animator.SetTrigger("jump");
        vfx.SendEvent("BallActivation");
    }

    private void OnDestroy()
    {
        SetIntensity(0f); // чтобы звук погас, пока подписчики ещё живы
        Destroyed?.Invoke();

        if (ball != null)
        {
            ball.Destroyed -= OnBallDestroy;
            ball.Animated -= OnBallAnimated;
            ball.Stopped -= OnBallStop;
            ball.Collided -= OnBallCollided;
        }
    }

    private void OnBallDestroy()
    {
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (ball == null) return;
        if (updateFrozen) return;

        transform.position = rouletteOrigin.position + ball.RelativeToRoulettePosition * scaleMultiplier;
        transform.rotation = ball.transform.rotation;
        transform.localScale = ball.transform.lossyScale * scaleMultiplier;

        UpdateRollIntensity();
    }

    private void UpdateRollIntensity()
    {
        var modelPos = ball.RelativeToRoulettePosition;

        if (!isRolling || !hasLastPosition || Time.deltaTime <= 0f)
        {
            lastModelPosition = modelPos;
            hasLastPosition = true;
            return;
        }

        // Скорость считаем в координатах физ. модели, чтобы не зависеть от scaleMultiplier.
        // Если шарик не касается поверхности (летит), качения нет.
        float speed = (modelPos - lastModelPosition).magnitude / Time.deltaTime;
        lastModelPosition = modelPos;

        float target = ball.IsInContact ? Mathf.Clamp01(speed / maxRollSpeed) : 0f;
        float smoothed = Mathf.Lerp(currentIntensity, target, 1f - Mathf.Exp(-intensitySmoothing * Time.deltaTime));

        // Хвост затухания дотягиваем до нуля, чтобы звук гарантированно выключился
        if (target == 0f && smoothed < intensityEpsilon) smoothed = 0f;

        SetIntensity(smoothed);
    }

    private void SetIntensity(float value)
    {
        if (value == currentIntensity) return;
        if (value != 0f && Mathf.Abs(value - currentIntensity) < intensityEpsilon) return;

        currentIntensity = value;
        RollIntensityChanged?.Invoke(Id, currentIntensity);
    }

    public void FreezeUpdate()
    {
        updateFrozen = !updateFrozen;
    }
}