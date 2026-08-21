using System;
using System.Linq;
using UnityEngine;
using static RoundStateManager;

public class WinningSequence
{
    public event Action<int, int, float> SequenceStart;
    public event Action<BetInfo, HitInfo> Highlight;
    public event Action<HitInfo> ApplyBall;
    public event Action<BetInfo, HitInfo> ApplyChips;
    public event Action<int, float, int> CountMoney;
    public event Action<bool> EndSequence;
    public event Action<float> DisplayMultiplier;

    private readonly BetTable betTable;
    private readonly RoundStateManager roundStateManager;
    private readonly BetCalculator betCalculator;

    private BetInfo betInfo;
    private HitInfo hitInfo;
    private bool isRoundWon;
    private int betSum;
    private float generalMultiplier;
    private float rerollCoeff;

    public WinningSequence(BetTable betTable, RoundStateManager roundStateManager, BetCalculator betCalculator)
    {
        this.betTable = betTable;
        this.roundStateManager = roundStateManager;
        this.betCalculator = betCalculator;
        betCalculator.EndRound += CallSequenceStart;
    }

    public void CallSequenceStart(HitInfo hitInfo)
    {
        if (!betTable.NotNullBetAmounts.Any()) return;

        isRoundWon = false;
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);
        CompareAndDisplayMaxMultiplier(0);

        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            SetInfo(betInfo, hitInfo, amount, betCalculator.RerollCoeff);
        }
        SequenceStart?.Invoke(betSum, 0, rerollCoeff);
    }

    public void CallPhaseExecution(SequencePhase sequencePhase)
    {
        switch (sequencePhase)
        {
            case SequencePhase.Init:
                break;
            case SequencePhase.BetHighlight:
                Highlight?.Invoke(betInfo, hitInfo);
                break;
            case SequencePhase.ApplyBall:
                isRoundWon = true;
                ApplyBall?.Invoke(hitInfo);
                break;
            case SequencePhase.ApplyChips:
                ApplyChips?.Invoke(betInfo, hitInfo);
                break;
            case SequencePhase.CountMoney:
                roundStateManager.ChangeState(RoundState.CountMoney);
                CountMoney?.Invoke(betSum, generalMultiplier, betCalculator.GetRewardForBet(betInfo, betSum, hitInfo));
                break;
            case SequencePhase.Finish:
                betCalculator.GetRewardAction(betCalculator.GetRewardForBet(betInfo, betSum, hitInfo));
                generalMultiplier = 0;
                EndSequence?.Invoke(isRoundWon);
                break;
        }
    }

    public void CompareAndDisplayMaxMultiplier(float multiplier)
    {
        generalMultiplier = Mathf.Max(generalMultiplier, multiplier);
        DisplayMultiplier?.Invoke(generalMultiplier);
    }

    public enum SequencePhase 
    {
        Init,
        BetHighlight,
        ApplyBall,
        ApplyChips,
        CountMoney,
        Finish
    }

    private void SetInfo(BetInfo betInfo, HitInfo hitInfo, int betSum, float rerollCoef)
    {
        this.hitInfo = hitInfo;
        this.betInfo = betInfo;
        this.betSum = betSum;
        this.rerollCoeff = rerollCoef;
    }
}
