public class FieldNumberMultiplierCard : Card
{
    private readonly int number;

    public FieldNumberMultiplierCard(BetCalculator betCalculator, FieldNumberMultiplierCardData data) : base(betCalculator, data)
    {
        this.number = data.Number;
    }

    public override void Apply()
    {
        BetCalculator.ModifyFieldNumberMultiplier(number, Multiplier);
    }
}
