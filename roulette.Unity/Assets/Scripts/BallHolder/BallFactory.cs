public class BallFactory
{
    private readonly BallDependencies deps;
    public BallFactory(
        Zenject.LazyInject<BetCalculator> betCalculator,
        Zenject.LazyInject<IRouletteWheel> rouletteWheel,
        Zenject.LazyInject<WinningSequence> winningSequence,
        Zenject.LazyInject<BallHolder> ballHolder)
    {
        deps = new(rouletteWheel, betCalculator, winningSequence, ballHolder);
    }

    public BallInfo Create(BallInfoData data) => data.CreateBall(System.Guid.NewGuid().ToString(), deps);
}
