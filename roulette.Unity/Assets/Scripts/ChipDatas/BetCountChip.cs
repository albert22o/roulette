public class BetCountChip : Chip
{
    private readonly TableInfo tableInfo;
    private readonly BetCalculator betCalculator;
    private readonly BetCountChipData stackChipData;

    public BetCountChip(TableInfo tableInfo, BetCalculator betCalculator, BetCountChipData stackChipData) : base(stackChipData)
    {
        this.tableInfo = tableInfo;
        this.betCalculator = betCalculator;
        this.stackChipData = stackChipData;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        tableInfo.AddToBetCount(stackChipData.BetCountDelta);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        tableInfo.AddToBetCount(-stackChipData.BetCountDelta);
    }
    public override void Apply()
    {
        betCalculator.ModifyGeneralModifier(stackChipData.BonusAmount);
    }
    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-stackChipData.BonusAmount);
    }
}