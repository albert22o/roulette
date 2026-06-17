public class BallFactory
{
    private readonly BallDependencies deps;

    public BallFactory()
    {
        deps = new ();
    }

    public BallInfo Create(BallInfoData data) => data.CreateBall(deps);
}
