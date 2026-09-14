using UnityEngine;

public class DotsChip : ChipHolderDependentChip
{
    private readonly int bonusPerChip;
    public DotsChip(BetCalculator betCalculator, ChipHolder chipHolder,
        DotsChipData data, BallResults ballResults, BetTable betTable) : base(betCalculator, chipHolder, data, ballResults, betTable) 
    {
        bonusPerChip = data.BonusPerChip;
    }

    public override float AppliedValue => Bonus;

    public override OperationType Operation => OperationType.Add;

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