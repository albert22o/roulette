public class NumberMultiplierCard : Card
{

    public NumberMultiplierCard(BetCalculator betCalculator, NumberMultiplierCardData data) : base(betCalculator, data)
    {

    }

    public override void Apply()
    {
        BetCalculator.ModifyFieldNumberMultiplier(Multiplier);
    }
}
