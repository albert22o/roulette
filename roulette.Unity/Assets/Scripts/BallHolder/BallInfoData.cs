using UnityEngine;

public abstract class BallInfoData : ScriptableObject
{
    public VisualComponent VisualComponent;
    public string Name;
    public string Description;
    public abstract BallInfo CreateBall(BallDependencies deps);
}