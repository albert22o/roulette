using UnityEngine;

public class BallVisual : MonoBehaviour
{
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
}
