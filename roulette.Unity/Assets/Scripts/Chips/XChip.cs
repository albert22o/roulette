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
        betCalculator.ModifyFieldNumberMultiplier(0, 1);
    }

    public override void Revert()
    {
        betCalculator.ModifyFieldNumberMultiplier(0, -1);
    }
}
