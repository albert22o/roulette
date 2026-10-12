using System;

public class FreeroamCursorHandler : IDisposable
{
    private readonly GameStateManager gameStateManager;
    private readonly CursorManager cursorManager;
    private bool isCursorVisible;

    public FreeroamCursorHandler(GameStateManager gameStateManager, CursorManager cursorManager)
    {
        this.gameStateManager = gameStateManager;
        this.cursorManager = cursorManager;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    public void Dispose()
    {
        if (gameStateManager != null)
        {
            gameStateManager.GameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.FreeRoam)
        {
            if (isCursorVisible)
            {
                cursorManager.HideCursorRequest();
                isCursorVisible = false;
            }
        }
        else
        {
            if (!isCursorVisible)
            {
                cursorManager.ShowCursorRequest();
                isCursorVisible = true;
            }
        }
    }
}
