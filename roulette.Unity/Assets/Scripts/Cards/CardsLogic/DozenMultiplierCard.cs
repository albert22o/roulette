using UnityEngine;

public class DozenMultiplierCard : Card
{
    public DozenMultiplierCard(BetCalculator betCalculator, DozenMultiplierCardData data) : base(betCalculator, data)
    {

    }

    public override void Apply()
    {
        BetCalculator.ModifyDozenMultiplier(Multiplier);
    }
}
