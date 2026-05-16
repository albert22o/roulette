public class ProgressionManager
{
    public RoundConfig Current => roundsConfig.Rounds[currentIndex];
    public int CurrentIndex => currentIndex;
    public bool HasNext => currentIndex < roundsConfig.Rounds.Count - 1;

    private readonly RoundsConfig roundsConfig;
    private int currentIndex = 0;

    public ProgressionManager(RoundsConfig roundsConfig, RoundStateManager roundStateManager)
    {
        this.roundsConfig = roundsConfig;
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        if (newState == RoundStateManager.RoundState.Win)
            Advance();
    }

    private void Advance()
    {
        if (!HasNext)
            return;

        currentIndex++;
    }
}