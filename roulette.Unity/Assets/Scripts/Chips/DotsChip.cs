using UnityEngine;

public class DotsChip : ChipHolderDependentChip
{
    private readonly int bonusPerChip;
    public DotsChip(BetCalculator betCalculator, DotsChipData data) : base(betCalculator, data) { }

    protected override int CalculateBonus()
    {
        return 0;
    }
}