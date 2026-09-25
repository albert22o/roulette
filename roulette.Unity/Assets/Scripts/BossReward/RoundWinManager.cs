using UnityEngine;

public class RoundWinManager
{
    private readonly RoundManager roundManager;
    private readonly BossReward bossReward;
    private readonly ProgressionManager progressionManager;
    private readonly RoundStateManager roundStateManager;

    public RoundWinManager(RoundManager roundManager, BossReward bossReward,
        ProgressionManager progressionManager, RoundStateManager roundStateManager, RewardCalculator rewardCalculator)
    {
        this.roundManager = roundManager;
        this.bossReward = bossReward;
        this.progressionManager = progressionManager;
        this.roundStateManager = roundStateManager;
        rewardCalculator.RewardReceived += OnRoundWin;
    }

    private void OnRoundWin()
    {
        if (progressionManager.Current.IsBoss)
        {
            bossReward.GiveReward();
        }
        else
        {
            roundStateManager.ChangeState(RoundStateManager.RoundState.Win);
        }
    }
}
