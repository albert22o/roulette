using UnityEngine;
using Zenject;

public class CursorManager : MonoBehaviour
{
    private GameStateManager gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        if (gameStateManager != null)
        {
            gameStateManager.GameStateChanged += OnGameStateChanged;
            UpdateCursorState(gameStateManager.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (gameStateManager != null)
        {
            gameStateManager.GameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        UpdateCursorState(state);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && gameStateManager != null)
        {
            UpdateCursorState(gameStateManager.CurrentState);
        }
    }

    private void UpdateCursorState(GameStateManager.GameState? state)
    {
        if (state == GameStateManager.GameState.FreeRoam)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
