using UnityEngine;
using Zenject;
using static GameStateManager;

public class BetButtonsController : MonoBehaviour
{
    private GameStateManager gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameStateManager.GameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        if (gameStateManager.CurrentState is not GameState.FieldsSelecting) gameObject.SetActive(true);
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state is GameState.FieldsSelecting)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
