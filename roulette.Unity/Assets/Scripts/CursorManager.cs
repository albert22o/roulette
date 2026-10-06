using UnityEngine;
using Zenject;

public class CursorManager : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private int cursorLockCount = 0;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        if (gameStateManager != null)
        {
            UpdateCursorState(gameStateManager.CurrentState);
        }
    }

    public void AddCursorLock()
    {
        cursorLockCount++;
        if (cursorLockCount > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RemoveCursorLock()
    {
        cursorLockCount--;
        cursorLockCount = Mathf.Max(0, cursorLockCount);
        if (cursorLockCount == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
            AddCursorLock();
        }
        else
        {
            RemoveCursorLock();
        }
    }
}
