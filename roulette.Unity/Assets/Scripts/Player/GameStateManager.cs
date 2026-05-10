using System;

public class GameStateManager
{
    public event Action<GameState> GameStateChanged;

    public void ChangeGameState(GameState state)
    {
        GameStateChanged?.Invoke(state);
    }

    public enum GameState
    {
        FreeRoam,
        BettingСycle
    }
}