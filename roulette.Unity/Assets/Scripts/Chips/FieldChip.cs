public class FieldChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int fieldNum;
    private readonly int modifyAmount;

    public FieldChip(BetCalculator betCalculator, FieldChipData fieldChipData) : base(fieldChipData) 
    {
        this.betCalculator = betCalculator;
        fieldNum = fieldChipData.FieldNum;
        modifyAmount = fieldChipData.ModifyAmount;
    }

    public override void Apply()
    {
        betCalculator.ModifyFieldNumberMultiplier(fieldNum, modifyAmount);
    }

    public override void Revert()
    {
        betCalculator.ModifyFieldNumberMultiplier(fieldNum, -modifyAmount);
    }
}
