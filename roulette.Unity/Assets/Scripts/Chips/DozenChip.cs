public class DozenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly DozenType dozenType;
    private readonly int modifyAmount;

    public DozenChip(BetCalculator betCalculator, DozenChipData dozenChipData) : base(dozenChipData)
    {
        this.betCalculator = betCalculator;
        dozenType = dozenChipData.DozenType;
        modifyAmount = dozenChipData.ModifyAmount;
    }

    public override void Apply()
    {
        betCalculator.ModifyDozenMultiplier(dozenType, modifyAmount);
    }

    public override void Revert()
    {
        betCalculator.ModifyDozenMultiplier(dozenType, -modifyAmount);
    }
}
