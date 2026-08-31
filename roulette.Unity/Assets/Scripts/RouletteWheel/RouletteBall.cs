using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class RouletteBall : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private OutlineController outline;
    [SerializeField] private BallVisual visual;

    private Transform rouletteOrigin;
    private Ball ball;
    private float scaleMultiplier;
    private bool isInteractionAvailable;

    private bool updateFrozen = false;
    private bool isMouseHover;

    public event Action<string> Hover;
    public event Action<string> UnHover;
    public event Action<string> Clicked;

    private Animator animator;
    public event Action Destroyed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    public void Initialize(Ball ball, float scaleMultiplier, Transform rouletteOrigin)
    {
        this.ball = ball;
        this.scaleMultiplier = scaleMultiplier;
        ball.Destroyed += OnBallDestroy;
        ball.Animated += OnBallAnimated;
        this.rouletteOrigin = rouletteOrigin;
        ball.Stopped += OnBallStop;
    }

    public void SetVisualComponent(VisualComponent visualComponent) => visual.SetVisualComponent(visualComponent);

    private void OnBallStop()
    {
        isInteractionAvailable = true;
        if (isMouseHover)
        {
            Hover?.Invoke(ball.Hash);
            outline.ToggleMaterial(true);
        }
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
        FreezeUpdate();
        transform.rotation = Quaternion.Euler(Vector3.up);
        animator.SetTrigger("jump");
    }

    private void OnDestroy()
    {
        Destroyed?.Invoke();
        if (ball != null)
        {
            ball.Destroyed -= OnBallDestroy;
            ball.Stopped -= OnBallStop;
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
    }

    public void FreezeUpdate()
    {
        updateFrozen = !updateFrozen;
    }

    public string GetBallHash()
    {
        return ball != null ? ball.Hash : null;
    }
}
