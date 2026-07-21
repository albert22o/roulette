public class GeneralMultiplierCard : Card
{
    public GeneralMultiplierCard(BetCalculator betCalculator, GeneralMultipilerCardData data) : base(betCalculator, data)
    {

    }

    public override void Apply()
    {
        BetCalculator.ModifyGeneralModifier(Multiplier);
    }
}
