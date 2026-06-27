using System;
using System.Collections.Generic;
using static RoundStateManager;

public class BetCalculator
{
    public event Action<int> GetReward;
    public event Action<HitInfo> EndRound;

    public int ZeroMultiplier { get; set; } = 10;
    public int NumMultiplier { get; set; } = 5;
    public int DozenMultiplier { get; set; } = 3;
    public int ColorMultiplier { get; set; } = 2;

    private readonly Dictionary<int, int> fieldNumberMultipliers = new();

    private readonly Dictionary<DozenType, int> dozenMultipliers = new();

    private readonly Dictionary<ColorType, int> colorMultipliers = new();

    private bool active = true;
    private readonly BetTable betTable;
    private int generalModifier = 0;

    public BetCalculator(BallResults ballResults, BetTable betTable, RoundStateManager roundStateManager)
    {
        this.betTable = betTable;
        ballResults.BallSelected += OnBallSelected;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void ModifyGeneralModifier(int delta)
    {
        generalModifier += delta;
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

        EndRound?.Invoke(info);
    }

    public int GetRewardForBet(BetInfo betInfo, int amount, HitInfo hitInfo)
    {
        return amount * Math.Max(GetMaxWinningMultiplier(betInfo, hitInfo), 0);
    }

    public int GetMaxWinningMultiplier(BetInfo betInfo, HitInfo hitInfo)
    {
        int multiplier = 0;

        if (betInfo.FieldNumber == hitInfo.FieldNumber)
        {
            multiplier = betInfo.FieldNumber == 0
                ? GetFieldMultiplier(0, ZeroMultiplier)
                : GetFieldMultiplier(hitInfo.FieldNumber, NumMultiplier);
        }

        if (betInfo.Dozen == hitInfo.Dozen)
            multiplier = Math.Max(multiplier, GetEffectiveDozenMultiplier(hitInfo.Dozen));

        if (betInfo.FieldColor == hitInfo.FieldColor)
            multiplier = Math.Max(multiplier, GetEffectiveColorMultiplier(hitInfo.FieldColor));

        multiplier += generalModifier;

        return multiplier;
    }

    public int GetFieldMultiplier(int fieldNumber, int defaultMultiplier) =>
        fieldNumberMultipliers.TryGetValue(fieldNumber, out var m) ? m : defaultMultiplier;

    public int GetEffectiveDozenMultiplier(DozenType dozen) =>
        dozenMultipliers.TryGetValue(dozen, out var m) ? m : DozenMultiplier;

    public int GetEffectiveColorMultiplier(ColorType color) =>
        colorMultipliers.TryGetValue(color, out var m) ? m : ColorMultiplier;

    public void GetRewardAction(int reward)
    {
        GetReward?.Invoke(reward);
    }
}