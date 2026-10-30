using System;

public class LoseManager
{
    public Action ExtraLifeTaken;
    public int ExtraLifes { get; set; } = 0;

    private readonly RoundManager roundManager;
    private readonly RoundStateManager roundStateManager;
    private readonly MenuMusicPlayer menuMusicPlayer;

    public LoseManager(RoundManager roundManager, RoundStateManager roundStateManager, MenuMusicPlayer menuMusicPlayer)
    {
        roundManager.Lose += OnLose;
        this.roundManager = roundManager;
        this.roundStateManager = roundStateManager;
        this.menuMusicPlayer = menuMusicPlayer;
    }

    private void OnLose()
    {
        if (ExtraLifes > 0)
        {
            ExtraLifes--;
            roundManager.ForceWin();
            ExtraLifeTaken?.Invoke();
            return;
        }

        menuMusicPlayer.Pause();
        roundStateManager.ChangeState(RoundStateManager.RoundState.Lose);
    }
}