using UnityEngine;

public class DozenMultiplierCard : Card
{
    private readonly DozenType dozen;

    public DozenMultiplierCard(BetCalculator betCalculator, DozenMultiplierCardData data) : base(betCalculator, data)
    {
        dozen = data.Dozen;
    }

    public override void Apply()
    {
        BetCalculator.ModifyDozenMultiplier(dozen, Multiplier);
    }
}
