public class BallDependencies
{
    private readonly Zenject.LazyInject<IRouletteWheel> rouletteWheel;
    private readonly Zenject.LazyInject<BetCalculator> betCalculator;
    private readonly Zenject.LazyInject<WinningSequence> winningSequence;
    private readonly Zenject.LazyInject<BallHolder> ballHolder;

    public IRouletteWheel RouletteWheel => rouletteWheel.Value;
    public BetCalculator BetCalculator => betCalculator.Value;
    public WinningSequence WinningSequence => winningSequence.Value;
    public BallHolder BallHolder => ballHolder.Value;

    public BallDependencies(
        Zenject.LazyInject<IRouletteWheel> rouletteWheel,
        Zenject.LazyInject<BetCalculator> betCalculator,
        Zenject.LazyInject<WinningSequence> winningSequence,
        Zenject.LazyInject<BallHolder> ballHolder)
    {
        this.rouletteWheel = rouletteWheel;
        this.betCalculator = betCalculator;
        this.winningSequence = winningSequence;
        this.ballHolder = ballHolder;
    }
}
