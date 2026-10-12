using System;
using System.Collections.Generic;
using System.Linq;
using static RoundStateManager;

public class BetCalculator
{
    public event Action<int> GetReward;
    public event Action<HitInfo> EndRound;
    public event Action MultipliersUpdated;

    private int zeroMultiplier = 10;
    private int numMultipler = 5;
    private int dozenMultipler = 3;
    private int colorMultiplier = 2;

    public int ZeroMultiplier
    {
        get { return zeroMultiplier; }
        set { zeroMultiplier = value; ZeroMultiplierChanged?.Invoke(zeroMultiplier); }
    }
    public int NumMultiplier
    {
        get { return numMultipler; }
        set { numMultipler = value; NumMultiplierChanged?.Invoke(numMultipler); }
    }
    public int DozenMultiplier
    {
        get { return dozenMultipler; }
        set { DozenMultiplier = value; DozenMultiplierChanged?.Invoke(dozenMultipler); }
    }
    public int ColorMultiplier
    {
        get { return colorMultiplier; }
        set { ColorMultiplier = value; ColorMultiplierChanged?.Invoke(colorMultiplier); }
    }

    public float RerollCoeff => rerollCoeff;

    private readonly HashSet<int> zeroFields = new HashSet<int>() { 0 };
    private readonly Dictionary<int, int> fieldNumberMultipliers = new();
    private readonly Dictionary<DozenType, int> dozenMultipliers = new();
    private readonly Dictionary<ColorType, int> colorMultipliers = new();

    private bool active = true;
    private List<(Func<float, float> Function, string OperationType, float Value)> modifySequence = new();
    private float rerollCoeff = 1.0f;
    private bool freeReroll = true;

    public event Action<float> RerollCoeffChanged;
    public event Action<int> NumMultiplierChanged;
    public event Action<int> DozenMultiplierChanged;
    public event Action<int> ColorMultiplierChanged;
    public event Action<int> ZeroMultiplierChanged;

    public BetCalculator(BallResults ballResults, RoundStateManager roundStateManager, RouletteWheel rouletteWheel)
    {
        rouletteWheel.BallClicked += (_) => ResetRerollCoeff();
        ballResults.BallSelected += OnBallSelected;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void DecreaseRerollCoeff()
    {
        if (freeReroll)
        {
            freeReroll = false;
            return;
        }
        rerollCoeff -= 0.25f;
        rerollCoeff = Math.Max(0, rerollCoeff);
        RerollCoeffChanged?.Invoke(rerollCoeff);
    }

    public void ResetRerollCoeff()
    {
        rerollCoeff = 1.0f;
        freeReroll = true;
        RerollCoeffChanged?.Invoke(rerollCoeff);
    }

    public void AddToZeroFields(int fieldNum) => zeroFields.Add(fieldNum);
    public void RemoveFromZeroFields(int fieldNum) => zeroFields.Remove(fieldNum);

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

    public void MultiplyGeneralModifier(float multiplier)
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
        MultipliersUpdated?.Invoke();
    }

    public void ModifyFieldNumberMultiplier(int delta)
    {
        for (int i = 1; i <= 36; i++)
            ModifyFieldNumberMultiplier(i, delta);
    }

    public void ModifyDozenMultiplier(DozenType dozen, int delta)
    {
        var current = dozenMultipliers.TryGetValue(dozen, out var m) ? m : DozenMultiplier;
        dozenMultipliers[dozen] = current + delta;
        MultipliersUpdated?.Invoke();
    }

    public void ModifyDozenMultiplier(int delta)
    {
        foreach (DozenType type in Enum.GetValues(typeof(DozenType)))
            ModifyDozenMultiplier(type, delta);
    }

    public void ModifyColorMultiplier(ColorType color, int delta)
    {
        var current = colorMultipliers.TryGetValue(color, out var m) ? m : ColorMultiplier;
        colorMultipliers[color] = current + delta;
        MultipliersUpdated?.Invoke();
    }

    public void ModifyColorMultiplier(int delta)
    {
        foreach (ColorType type in Enum.GetValues(typeof(ColorType)))
            ModifyColorMultiplier(type, delta);
    }

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }

    private void OnBallSelected(HitInfo info)
    {
        if (info == null)
        {
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