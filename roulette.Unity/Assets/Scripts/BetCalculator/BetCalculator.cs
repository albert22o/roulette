using System;
using System.Collections.Generic;
using static RoundStateManager;

public class BetCalculator
{
    public event Action<int> GetReward;
    public event Action<HitInfo> HighlightButtons;

    public int ZeroMultiplier { get; set; } = 10;
    public int NumMultiplier { get; set; } = 5;
    public int DozenMultiplier { get; set; } = 3;
    public int ColorMultiplier { get; set; } = 2;

    private readonly Dictionary<int, int> fieldNumberMultipliers = new();

    private readonly Dictionary<DozenType, int> dozenMultipliers = new();

    private readonly Dictionary<ColorType, int> colorMultipliers = new();

    private bool active = true;
    private readonly BetTable betTable;
    private int reward;

    public BetCalculator(BallResults ballResults, BetTable betTable, RoundStateManager roundStateManager)
    {
        this.betTable = betTable;
        ballResults.BallSelected += OnBallSelected;
        roundStateManager.StateChanged += OnStateChanged;
    }


    public void ModifyFieldNumberMultiplier(int fieldNumber, int delta)
    {
        var current = fieldNumberMultipliers.TryGetValue(fieldNumber, out var m)
            ? m
            : (fieldNumber == 0 ? ZeroMultiplier : NumMultiplier);
        fieldNumberMultipliers[fieldNumber] = current + delta;
    }

    public void ModifyDozenMultiplier(DozenType dozen, int delta)
    {
        var current = dozenMultipliers.TryGetValue(dozen, out var m) ? m : DozenMultiplier;
        dozenMultipliers[dozen] = current + delta;
    }

    public void ModifyColorMultiplier(ColorType color, int delta)
    {
        var current = colorMultipliers.TryGetValue(color, out var m) ? m : ColorMultiplier;
        colorMultipliers[color] = current + delta;
    }

    public void ResetFieldNumberMultiplier(int fieldNumber) =>
        fieldNumberMultipliers.Remove(fieldNumber);

    public void ResetDozenMultiplier(DozenType dozen) =>
        dozenMultipliers.Remove(dozen);

    public void ResetColorMultiplier(ColorType color) =>
        colorMultipliers.Remove(color);

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }

    private void OnBallSelected(HitInfo info)
    {
        if (!active)
            return;

        HighlightButtons?.Invoke(info);
    }

    private int GetRewardForBet(BetInfo betInfo, int amount, HitInfo hitInfo)
    {
        if (betInfo.FieldNumber == hitInfo.FieldNumber)
        {
            int multiplier = betInfo.FieldNumber == 0
                ? GetFieldMultiplier(0, ZeroMultiplier)
                : GetFieldMultiplier(hitInfo.FieldNumber, NumMultiplier);

            return amount * Math.Max(multiplier, 0);
        }

        if (betInfo.Dozen == hitInfo.Dozen)
            return amount * Math.Max(GetEffectiveDozenMultiplier(hitInfo.Dozen), 0);

        if (betInfo.FieldColor == hitInfo.FieldColor)
            return amount * Math.Max(GetEffectiveColorMultiplier(hitInfo.FieldColor), 0);

        return amount;
    }

    public int GetFieldMultiplier(int fieldNumber, int defaultMultiplier) =>
        fieldNumberMultipliers.TryGetValue(fieldNumber, out var m) ? m : defaultMultiplier;

    public int GetEffectiveDozenMultiplier(DozenType dozen) =>
        dozenMultipliers.TryGetValue(dozen, out var m) ? m : DozenMultiplier;

    public int GetEffectiveColorMultiplier(ColorType color) =>
        colorMultipliers.TryGetValue(color, out var m) ? m : ColorMultiplier;

    public void GetRewardAction(HitInfo info)
    {
        reward = 0;
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
            reward += GetRewardForBet(betInfo, amount, info);

        GetReward?.Invoke(reward);
    }
}