using UnityEngine;

// Базовый класс для фишек, которые считают что-то в ChipHolder
public abstract class ChipHolderDependentChip : Chip
{
    protected readonly BetCalculator betCalculator;
    protected readonly ChipHolder chipHolder;
    private bool bonusApplied = false;
    protected int Bonus;

    protected ChipHolderDependentChip(BetCalculator betCalculator, ChipHolder chipHolder, ChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
    }

    protected abstract int CalculateBonus();

    public override bool Apply()
    {
        if (bonusApplied) return false;
        Bonus = CalculateBonus();
        if (Bonus > 0)
        {
            betCalculator.ModifyGeneralModifier(Bonus);
            bonusApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (!bonusApplied) return;
        int bonus = CalculateBonus();
        if (bonus > 0)
        {
            betCalculator.ModifyGeneralModifier(-bonus);
        }
        bonusApplied = false;
    }
}