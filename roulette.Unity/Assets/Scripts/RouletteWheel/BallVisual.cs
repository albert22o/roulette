using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public event Action<string> Hover;
    public event Action<string> UnHover;
    public event Action<string> Clicked;
    public event Action Destroyed;

    [SerializeField] private Renderer meshRenderer;
    private Transform rouletteOrigin;
    private Ball ball;
    private VisualComponent visualComponent;
    private float scaleMultiplier;

    public void SetVisualComponent(VisualComponent visualComponent)
    {
        this.visualComponent = visualComponent;
        meshRenderer.material = visualComponent.Material;
    }

    public void Initialize(Ball ball, float scaleMultiplier, Transform rouletteOrigin)
    {
        this.ball = ball;
        this.scaleMultiplier = scaleMultiplier;
        ball.Destroyed += OnBallDestroy;
        this.rouletteOrigin = rouletteOrigin;
    }

    private void OnDestroy()
    {
        Destroyed?.Invoke();
        if (ball != null)
        {
            ball.Destroyed -= OnBallDestroy;
        }
    }

    private void OnBallDestroy()
    {
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (ball == null) return;

        transform.position = rouletteOrigin.position + ball.RelativeToRoulettePosition * scaleMultiplier;

        transform.rotation = ball.transform.rotation;

        transform.localScale = ball.transform.lossyScale * scaleMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover?.Invoke(ball.Hash);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnHover?.Invoke(ball.Hash);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(ball.Hash);
    }
}
