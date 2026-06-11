using UnityEngine;

public class XChip : Chip
{
    private readonly BetCalculator betCalculator;

    public XChip(BetCalculator betCalculator, XChipData data) : base(data)  
    {
        this.betCalculator = betCalculator;
    }

    public override void Apply()
    {
        for (int i = 0; i <= 36; i++)
            betCalculator.ModifyFieldNumberMultiplier(i, 1);
    }

    public override void Revert()
    {
        for (int i = 0; i <= 36; i++)
            betCalculator.ModifyFieldNumberMultiplier(i, -1);
    }
}
