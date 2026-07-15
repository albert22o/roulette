using UnityEngine;

public abstract class BallInfoData : ItemData
{
    public VisualComponent VisualComponent;
    public Sprite Icon;
    public abstract BallInfo CreateBall(BallDependencies deps);
}