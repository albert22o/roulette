using System;
using System.Collections.Generic;

public class GameStateManager
{
    public event Action<GameState> GameStateChanged;

    private GameState? currentState = GameState.FreeRoam;
    private readonly Queue<GameState> stateQueue = new Queue<GameState>();
    private bool isProcessing;

    public GameState? CurrentState => currentState;

    public void ChangeGameState(GameState state)
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
            GameStateChanged?.Invoke((GameState)currentState);
        }

        isProcessing = false;
    }

    public enum GameState
    {
        FreeRoam,
        BettingСycle,
        Shopping,
        Lose,
        PlaqueShopping,
        FieldsSelecting,
        BossReward,
        Win,
    }
}