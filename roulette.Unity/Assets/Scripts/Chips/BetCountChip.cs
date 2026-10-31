public class BetCountChip : Chip
{
    private readonly TableInfo tableInfo;
    private readonly BetCountChipData stackChipData;
    private readonly BallResults ballResults;

    public override float AppliedValue => stackChipData.BonusAmount;
    public override OperationType Operation => OperationType.Add;

    public BetCountChip(TableInfo tableInfo, BetCalculator betCalculator,
        BetCountChipData stackChipData, BetTable betTable, BallResults ballResults) : base(stackChipData)
    {
        this.tableInfo = tableInfo;
        this.stackChipData = stackChipData;
        this.ballResults = ballResults;

        this.ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    { 
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
    public override bool Apply()
    {
        return false;
    }
    public override void Revert()
    {
    }
}