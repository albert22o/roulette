using UnityEngine;

public class ColorMultiplierCard : Card
{
    public ColorMultiplierCard(BetCalculator betCalculator, ColorMultipilerCardData data) : base(betCalculator, data)
    {
        
    }

    public override void Apply()
    {
        BetCalculator.ModifyColorMultiplier(Multiplier);
    }
}
