using UnityEngine;

public class XplusChip : Chip
{
    private readonly BetCalculator betCalculator;
    public XplusChip(BetCalculator betCalculator, XplusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
    }

    public override void Apply()
    {
        betCalculator.ModifyFieldNumberMultiplier(0, 2);
    }

    public override void Revert()
    {
        betCalculator.ModifyFieldNumberMultiplier(0, -2);
    }
}
    

