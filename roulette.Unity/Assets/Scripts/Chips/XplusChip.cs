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
        for (int i = 0; i <= 36; i++)
            betCalculator.ModifyFieldNumberMultiplier(i, 2);
    }

    public override void Revert()
    {
        for (int i = 0; i <= 36; i++)
            betCalculator.ModifyFieldNumberMultiplier(i, -2);
    }
}
    

