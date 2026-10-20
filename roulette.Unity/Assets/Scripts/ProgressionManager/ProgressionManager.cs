public class ProgressionManager
{
    public RoundConfig Current => roundsConfig.Rounds[currentIndex];
    public int CurrentIndex => currentIndex;
    public bool HasNext => currentIndex < roundsConfig.Rounds.Count - 1;

    private readonly RoundsConfig roundsConfig;
    private readonly GameStateManager gameStateManager;
    private int currentIndex = 0;

    public ProgressionManager(RoundsConfig roundsConfig, RoundStateManager roundStateManager, GameStateManager gameStateManager)
    {
        this.roundsConfig = roundsConfig;
        this.gameStateManager = gameStateManager;
    }

    public void Advance()
    {
        if (!HasNext)
        {
            gameStateManager.ChangeGameState(GameStateManager.GameState.Win);
            return;
        }

        currentIndex++;
    }
}
