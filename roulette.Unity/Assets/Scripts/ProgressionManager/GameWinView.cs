using System;
using UnityEngine;
using Zenject;

public class GameWinView : MonoBehaviour
{
    [SerializeField] GameObject gameWinScreen;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Win)
            gameWinScreen.SetActive(true);
    }
}