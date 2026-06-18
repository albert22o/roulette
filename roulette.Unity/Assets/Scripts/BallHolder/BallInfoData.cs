public abstract class BallInfoData : ItemData
{
    public VisualComponent VisualComponent;

    public abstract BallInfo CreateBall(BallDependencies deps);
}