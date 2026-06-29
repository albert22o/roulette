using UnityEngine;
using Zenject;

public class GameOverView : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        gameOverPanel.SetActive(newState == GameStateManager.GameState.Lose);
    }
}
