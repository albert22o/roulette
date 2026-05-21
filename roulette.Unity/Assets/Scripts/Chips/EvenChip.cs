public class EvenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int modifyAmount;

    public EvenChip(BetCalculator betCalculator, EvenChipData evenChipData) : base(evenChipData) 
    {
        this.betCalculator = betCalculator;
        modifyAmount = evenChipData.ModifyAmount;
    }

    public override void Apply()
    {
        for (var i = 2; i <= 36; i += 2)
            betCalculator.ModifyFieldNumberMultiplier(i, modifyAmount);
    }

    public override void Revert()
    {
        for (var i = 2; i <= 36; i += 2)
            betCalculator.ModifyFieldNumberMultiplier(i, -modifyAmount);
    }
}
