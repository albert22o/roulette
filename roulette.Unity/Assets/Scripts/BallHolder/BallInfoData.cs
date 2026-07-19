using UnityEngine;

public abstract class BallInfoData : ItemData
{
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private VisualComponent visualComponent;

    public VisualComponent VisualComponent => visualComponent;
    public Ball BallPrefab => ballPrefab;

    public abstract BallInfo CreateBall(string hash, BallDependencies deps);
}