using UnityEngine;

public class ColorMultiplierCard : Card
{
    private readonly ColorType color;

    public ColorMultiplierCard(BetCalculator betCalculator, ColorMultipilerCardData data) : base(betCalculator, data)
    {
        color = data.Color;
    }

    public override void Apply()
    {
        BetCalculator.ModifyColorMultiplier(color, Multiplier);
    }
}
