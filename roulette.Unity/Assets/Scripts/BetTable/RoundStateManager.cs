using System;

public class RoundStateManager
{
    private readonly GameStateManager gameStateManager;

    public event Action<BetState> StateChanged;

    public RoundStateManager(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.BettingСycle)
            ChangeState(BetState.PlacingBets);
        else
            ChangeState(BetState.NotActive);
    }

    public void ChangeState(BetState state)
    {
        StateChanged?.Invoke(state);
    }

    public enum BetState
    {
        PlacingBets,
        WheelSpining,
        ChoosingReward,
        NotActive
    }
}