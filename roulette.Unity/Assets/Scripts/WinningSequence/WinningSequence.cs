using System;
using System.Linq;
using UnityEngine;

public class WinningSequence
{
    public event Action<int, int> SequenceStart;
    public event Action<BetInfo, HitInfo> Highlight;
    public event Action<HitInfo> ApplyBall;
    public event Action<BetInfo, HitInfo> ApplyChips;
    public event Action<int, int, int> CountMoney;
    public event Action<bool> EndSequence;
    public event Action<int> DisplayMultiplier;

    private readonly BetTable betTable;
    private readonly RoundStateManager roundStateManager;
    private BetCalculator betCalculator;

    private SequencePhase phase;
    private BetInfo betInfo;
    private HitInfo hitInfo;
    private bool isRoundWon;
    private int betSum;
    private int generalMultiplier;

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

        phase = SequencePhase.Init;
        isRoundWon = false;
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);
        CompareAndDisplayMaxMultiplier(1);

        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            SetInfo(betInfo, hitInfo, amount);
        }
        SequenceStart?.Invoke(betSum, 0);
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
                CountMoney?.Invoke(betSum, generalMultiplier, betCalculator.GetRewardForBet(betInfo, betSum, hitInfo));
                break;
            case SequencePhase.Finish:
                betCalculator.GetRewardAction(betCalculator.GetRewardForBet(betInfo, betSum, hitInfo));
                generalMultiplier = 0;
                betTable.ClearBets();
                EndSequence?.Invoke(isRoundWon);
                break;
        }
    }

    public void CompareAndDisplayMaxMultiplier(int multiplier)
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

    private void SetInfo(BetInfo betInfo, HitInfo hitInfo, int betSum)
    {
        this.hitInfo = hitInfo;
        this.betInfo = betInfo;
        this.betSum = betSum;
    }
}
