using System;
using System.Collections.Generic;

public class RoundStateManager
{
    public event Action<RoundState> StateChanged;

    private readonly GameStateManager gameStateManager;
    private readonly Queue<RoundState> stateQueue = new();
    private bool isProcessing;
    private RoundState? currentState = null;
    private bool wasInRunProgress = false;

    public RoundStateManager(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    public void ChangeState(RoundState state)
    {
        stateQueue.Enqueue(state);

        if (isProcessing)
            return;

        isProcessing = true;

        while (stateQueue.Count > 0)
        {
            var nextState = stateQueue.Dequeue();

            if (currentState == nextState)
                continue;

            currentState = nextState;
            StateChanged?.Invoke((RoundState)currentState); 

            if (currentState == RoundState.Lose)
                gameStateManager.ChangeGameState(GameStateManager.GameState.Lose);
            else if (currentState == RoundState.Win)
                gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
        }

        isProcessing = false;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.RunProgress)
        {
            wasInRunProgress = true;
            return;
        }
        if (wasInRunProgress)
        {
            wasInRunProgress = false;
            return;
        }
        if (state == GameStateManager.GameState.BettingСycle)
            ChangeState(RoundState.Start);
        else
            ChangeState(RoundState.NotActive);
    }

    public enum RoundState
    {
        Start,
        PlacingBets,
        WheelSpining,
        ChoosingReward,
        BetResults,
        Lose,
        Win,
        NotActive
    }
}