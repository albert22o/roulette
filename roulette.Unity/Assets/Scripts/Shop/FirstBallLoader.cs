using UnityEngine;
using Zenject;

public class FirstBallLoader : MonoBehaviour
{
    private BallHolder ballHolder;
    [SerializeField] private BallInfoData ball;

    [Inject]
    public void Construct(BallHolder ballHolder)
    {
        this.ballHolder = ballHolder;
    }

    public void LoadBall()
    {
        ballHolder.AddBall(ball);
    }
}
