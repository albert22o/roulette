using UnityEngine;

public class DotsBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly int bonusPerChip;

    private bool modifierApplied = false;
    private int appliedBonus = 0;

    public DotsBonusChip(
        BetCalculator betCalculator,
        ChipHolder chipHolder,
        DotsBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        bonusPerChip = data.BonusPerChip;
    }

    public override void Apply()
    {
        int otherChipsCount = 0;
        for (int i = 0; i < chipHolder.Сhips.Length; i++)
        {
            if (chipHolder.Сhips[i] != null && chipHolder.Сhips[i] != this)
            {
                otherChipsCount++;
            }
        }

        appliedBonus = otherChipsCount * bonusPerChip;

        if (appliedBonus > 0)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, appliedBonus);
            }

            modifierApplied = true;
        }
    }

    public override void Revert()
    {
        if (modifierApplied)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, -appliedBonus);
            }

            modifierApplied = false;
            appliedBonus = 0;
        }
    }
}