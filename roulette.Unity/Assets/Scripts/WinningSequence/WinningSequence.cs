using System;
using System.Linq;
using UnityEngine;
using static RoundStateManager;

public enum EndSequenceType
{
    WinGame,
    LoseGame,
    WinRound,
    FinishBet
    
}

public class WinningSequence
{
    public event Action<int, int, float> SequenceStart;
    public event Action<BetInfo, HitInfo> Highlight;
    public event Action<BetInfo, HitInfo> ApplyBall;
    public event Action<BetInfo, HitInfo> ApplyChips;
    public event Action<int, float, int> CountMoney;
    public event Action<EndSequenceType> EndSequence;
    public event Action<float> DisplayMultiplier;
    public event Action<float> MoneyCountStarted;
    public event Action<BetButton.HighlightPhase> StageMatched;

    private readonly BetTable betTable;
    private readonly RoundStateManager roundStateManager;
    private readonly BetCalculator betCalculator;
    private EndSequenceType endSequenceType;

    private BetInfo betInfo;
    private HitInfo hitInfo;
    private int betSum;
    private float generalMultiplier;
    private float rerollCoeff;

    public WinningSequence(BetTable betTable, RoundStateManager roundStateManager,
        GameStateManager gameStateManager, BetCalculator betCalculator,
        RoundManager roundManager)
    {
        this.betTable = betTable;
        this.roundStateManager = roundStateManager;
        this.betCalculator = betCalculator;
        betCalculator.EndRound += CallSequenceStart;
        roundStateManager.StateChanged += OnStateChanged;
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    // Вызывать из подсветки, когда этап ДЕЙСТВИТЕЛЬНО совпал
    public void NotifyStageMatched(BetButton.HighlightPhase phase)
    {
        StageMatched?.Invoke(phase);
    }

    public void NotifyMoneyCountStarted(float duration)
    {
        MoneyCountStarted?.Invoke(duration);
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Win)
        {
            endSequenceType = EndSequenceType.WinGame;
        }
        else if (state == GameStateManager.GameState.Lose)
        {
            endSequenceType = EndSequenceType.LoseGame;
        }
    }

    private void OnStateChanged(RoundState state)
    {
        if (state == RoundState.Win)
        {
            endSequenceType = EndSequenceType.WinRound;
        }
    }

    public void CallSequenceStart(HitInfo hitInfo)
    {
        if (!betTable.NotNullBetAmounts.Any()) return;

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
        endSequenceType = EndSequenceType.FinishBet;
        switch (sequencePhase)
        {
            case SequencePhase.Init:
                break;
            case SequencePhase.BetHighlight:
                Highlight?.Invoke(betInfo, hitInfo);
                break;
            case SequencePhase.ApplyBall:
                ApplyBall?.Invoke(betInfo, hitInfo);
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
                betCalculator.ResetRerollCoeff();
                EndSequence?.Invoke(endSequenceType);
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
