using System;
using UnityEngine;
using Zenject;

public class GameWinView : MonoBehaviour
{
    [SerializeField] GameObject gameWinScreen;

    private MenuMusicPlayer menuMusicPlayer;

    [Inject]
    public void Construct(GameStateManager gameStateManager, MenuMusicPlayer menuMusicPlayer)
    {
        gameStateManager.GameStateChanged += OnGameStateChanged;
        this.menuMusicPlayer = menuMusicPlayer;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Win)
        {
            menuMusicPlayer.Pause();
            gameWinScreen.SetActive(true);
        }
    }
}