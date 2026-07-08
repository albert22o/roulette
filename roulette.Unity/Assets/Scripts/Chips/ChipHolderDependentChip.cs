using UnityEngine;

// Базовый класс для фишек, которые считают что-то в ChipHolder
public abstract class ChipHolderDependentChip : Chip
{
    protected readonly BetCalculator betCalculator;
    protected readonly ChipHolder chipHolder;
    private bool bonusApplied = false;

    protected ChipHolderDependentChip(BetCalculator betCalculator, ChipHolder chipHolder, ChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
    }

    protected abstract int CalculateBonus();

    public override void Apply()
    {
        if (bonusApplied) return;
        int bonus = CalculateBonus();
        if (bonus > 0)
        {
            betCalculator.ModifyGeneralModifier(bonus);
            bonusApplied = true;
        }
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