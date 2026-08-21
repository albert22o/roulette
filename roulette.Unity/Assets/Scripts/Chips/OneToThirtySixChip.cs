public class OneToThirtySixChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int[] pseudoZeroNumbers = { 1, 2, 35, 36 };

    public OneToThirtySixChip(BetCalculator betCalculator, OneToThirtySixChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
    }

    public override bool Apply()
    {
        foreach (int number in pseudoZeroNumbers)
            betCalculator.AddToZeroFields(number);
        return true;
    }

    public override void Revert()
    {
        foreach (int number in pseudoZeroNumbers)
            betCalculator.RemoveFromZeroFields(number);
    }
}