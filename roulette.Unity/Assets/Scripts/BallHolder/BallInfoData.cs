using UnityEngine;

public abstract class BallInfoData : ScriptableObject
{
    public VisualComponent VisualComponent;

    public abstract BallInfo CreateBall(BallDependencies deps);
}