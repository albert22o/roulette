using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BallVisual : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    private Ball ball;
    private VisualComponent visualComponent;
    private float scaleMultiplier;




    public void SetVisualComponent(VisualComponent visualComponent)
    {
        this.visualComponent = visualComponent;
        renderer.material = visualComponent.Material;
    }

    public void Initialize(Ball ball, float scaleMultiplier)
    {
        this.ball = ball;
        this.scaleMultiplier = scaleMultiplier;
        ball.Destroyed += OnBallDestroy;
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

        transform.position = ball.transform.position * scaleMultiplier;

        transform.rotation = ball.transform.rotation;

        transform.localScale = ball.transform.lossyScale * scaleMultiplier;
    }
}
