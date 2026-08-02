using UnityEngine;

public abstract class BallInfoData : ItemData
{
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private VisualComponent visualComponent;
    [SerializeField] private bool isDestroyable = false;

    public VisualComponent VisualComponent => visualComponent;
    public Ball BallPrefab => ballPrefab;
    public bool IsDestroyable => isDestroyable;

    public abstract BallInfo CreateBall(string hash, BallDependencies deps);
}