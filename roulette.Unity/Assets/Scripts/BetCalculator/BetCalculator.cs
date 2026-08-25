using System;
using System.Collections.Generic;
using System.Linq;
using static RoundStateManager;

public class BetCalculator
{
    public event Action<int> GetReward;
    public event Action<HitInfo> EndRound;

    public int ZeroMultiplier { get; set; } = 10;
    public int NumMultiplier { get; set; } = 5;
    public int DozenMultiplier { get; set; } = 3;
    public int ColorMultiplier { get; set; } = 2;
    public float RerollCoeff => rerollCoeff;

    private readonly HashSet<int> zeroFields = new HashSet<int>() { 0 };

    private readonly Dictionary<int, int> fieldNumberMultipliers = new();

    private readonly Dictionary<DozenType, int> dozenMultipliers = new();

    private readonly Dictionary<ColorType, int> colorMultipliers = new();

    private bool active = true;

    private List<(Func<float, float> Function, string OperationType, float Value)> modifySequence = new();

    private float rerollCoeff = 1.0f;

    public event Action<float> RerollCoeffChanged;

    public BetCalculator(BallResults ballResults, RoundStateManager roundStateManager)
    {
        ballResults.BallSelected += OnBallSelected;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void DecreaseRerollCoeff()
    {
        rerollCoeff -= 0.25f;
        rerollCoeff = Math.Max(0, rerollCoeff);
        RerollCoeffChanged?.Invoke(rerollCoeff);
    }

    public void ResetRerollCoeff()
    {
        rerollCoeff = 1.0f;
        RerollCoeffChanged?.Invoke(rerollCoeff);
    }

    public void AddToZeroFields(int fieldNum)
    {
        zeroFields.Add(fieldNum);
    }

    public void RemoveFromZeroFields(int fieldNum)
    {
        zeroFields.Remove(fieldNum);
    }

    public void ModifyGeneralModifier(int delta)
    {
        var inverseIndex = modifySequence.FindLastIndex(item =>
            item.OperationType == "Add" && Math.Abs(item.Value - (-delta)) < 0.0001f);

        if (inverseIndex >= 0)
        {
            modifySequence.RemoveAt(inverseIndex);
            return;
        }

        modifySequence.Add((x => x + delta, "Add", delta));
    }

    public void MuliplyGeneralModifier(float multiplier)
    {
        var inverseIndex = modifySequence.FindLastIndex(item =>
            item.OperationType == "Divide" && Math.Abs(item.Value - multiplier) < 0.0001f);

        if (inverseIndex >= 0)
        {
            modifySequence.RemoveAt(inverseIndex);
            return;
        }

        modifySequence.Add((x => (x * multiplier), "Multiply", multiplier));
    }

    public void DivideGeneralModifier(float divisor)
    {
        var inverseIndex = modifySequence.FindLastIndex(item =>
            item.OperationType == "Multiply" && Math.Abs(item.Value - divisor) < 0.0001f);

        if (inverseIndex >= 0)
        {
            modifySequence.RemoveAt(inverseIndex);
            return;
        }

        modifySequence.Add((x => (int)(x / divisor), "Divide", divisor));
    }

    public void ModifyFieldNumberMultiplier(int fieldNumber, int delta)
    {
        var current = fieldNumberMultipliers.TryGetValue(fieldNumber, out var m)
            ? m
            : (fieldNumber == 0 ? ZeroMultiplier : NumMultiplier);
        fieldNumberMultipliers[fieldNumber] = current + delta;
    }

    public void ModifyFieldNumberMultiplier(int delta)
    {
        for (int i = 1; i <= 36; i++)
        {
            ModifyFieldNumberMultiplier(i, delta);
        }
    }

    public void ModifyDozenMultiplier(DozenType dozen, int delta)
    {
        var current = dozenMultipliers.TryGetValue(dozen, out var m) ? m : DozenMultiplier;
        dozenMultipliers[dozen] = current + delta;
    }

    public void ModifyDozenMultiplier(int delta)
    {
        foreach (DozenType type in Enum.GetValues(typeof(DozenType)))
        {
            ModifyDozenMultiplier(type, delta);
        }
    }

    public void ModifyColorMultiplier(ColorType color, int delta)
    {
        var current = colorMultipliers.TryGetValue(color, out var m) ? m : ColorMultiplier;
        colorMultipliers[color] = current + delta;
    }

    public void ModifyColorMultiplier(int delta)
    {
        foreach (ColorType type in Enum.GetValues(typeof(ColorType)))
        {
            ModifyColorMultiplier(type, delta);
        }
    }

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }

    private void OnBallSelected(HitInfo info)
    {
        if (info == null)
        {
            // Если информации о ставке не пришло - автопроигрыш
            GetReward?.Invoke(0);
            return;
        }
        if (!active)
            return;
        EndRound?.Invoke(info);
    }

    public int GetRewardForBet(BetInfo betInfo, int amount, HitInfo hitInfo)
    {
        return (int)(amount * Math.Max(GetMaxWinningMultiplier(betInfo, hitInfo), 0) * rerollCoeff);
    }

    public float GetMaxWinningMultiplier(BetInfo betInfo, HitInfo hitInfo)
    {
        float multiplier = 0;

        if (betInfo.FieldNumber == 0 && zeroFields.Contains(hitInfo.FieldNumber))
            multiplier = Math.Max(multiplier, ZeroMultiplier);

        if (betInfo.FieldNumber == hitInfo.FieldNumber)
            multiplier = Math.Max(multiplier, GetFieldMultiplier(hitInfo.FieldNumber, NumMultiplier));

        if (betInfo.Dozen == hitInfo.Dozen)
            multiplier = Math.Max(multiplier, GetEffectiveDozenMultiplier(hitInfo.Dozen));

        if (betInfo.FieldColor == hitInfo.FieldColor)
            multiplier = Math.Max(multiplier, GetEffectiveColorMultiplier(hitInfo.FieldColor));

        if (multiplier == 0)
            return 0;

        multiplier = ApplyModifiers(multiplier);

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
    private float ApplyModifiers(float value)
    {
        return modifySequence.Aggregate(value, (current, item) => item.Function(current));
    }
}