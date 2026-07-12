using System;

public class LoseManager
{
    public Action ExtraLifeTaken;
    public int ExtraLifes { get; set; } = 0;

    private readonly RoundManager roundManager;
    private readonly RoundStateManager roundStateManager;

    public LoseManager(RoundManager roundManager, RoundStateManager roundStateManager)
    {
        roundManager.Lose += OnLose;
        this.roundManager = roundManager;
        this.roundStateManager = roundStateManager;
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

        roundStateManager.ChangeState(RoundStateManager.RoundState.Lose);
    }
}