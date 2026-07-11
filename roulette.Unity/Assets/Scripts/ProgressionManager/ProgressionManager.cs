public class ProgressionManager
{
    public RoundConfig Current => roundsConfig.Rounds[currentIndex];
    public int CurrentIndex => currentIndex;
    public bool HasNext => currentIndex < roundsConfig.Rounds.Count - 1;

    private readonly RoundsConfig roundsConfig;
    private int currentIndex = 0;

    public ProgressionManager(RoundsConfig roundsConfig)
    {
        this.roundsConfig = roundsConfig;
    }

    public void Advance()
    {
        if (!HasNext)
            return;

        currentIndex++;
    }
}