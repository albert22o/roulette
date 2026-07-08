using UnityEngine;

public class DotsChip : ChipHolderDependentChip
{
    private readonly int bonusPerChip;
    public DotsChip(BetCalculator betCalculator, ChipHolder chipHolder, DotsChipData data) : base(betCalculator, chipHolder, data) 
    {
        bonusPerChip = data.BonusPerChip;
    }

    protected override int CalculateBonus()
    {
        var chipCount = 0;
        foreach (var chip in chipHolder.Chips)
        {
            if (chip == null)
                continue;
            if (chip is DotsChip)
                continue;
            chipCount++;
        }
        return chipCount * bonusPerChip;
    }
}