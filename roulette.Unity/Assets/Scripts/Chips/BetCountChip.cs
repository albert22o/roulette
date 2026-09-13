public class BetCountChip : Chip
{
    private readonly TableInfo tableInfo;
    private readonly BetCalculator betCalculator;
    private readonly BetCountChipData stackChipData;
    private readonly BetTable betTable;
    private bool applyConditionsMatched;
    private bool multiplierApplied = false;
    private readonly BallResults ballResults;

    public override int AppliedMultiplier => stackChipData.BonusAmount;

    public BetCountChip(TableInfo tableInfo, BetCalculator betCalculator,
        BetCountChipData stackChipData, BetTable betTable, BallResults ballResults) : base(stackChipData)
    {
        this.tableInfo = tableInfo;
        this.betCalculator = betCalculator;
        this.stackChipData = stackChipData;
        this.betTable = betTable;
        this.ballResults = ballResults;

        this.ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit);
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
        if (applyConditionsMatched)
        {
            betCalculator.ModifyGeneralModifier(stackChipData.BonusAmount);
            multiplierApplied = true;
            return true;
        }
        return false;
    }
    public override void Revert()
    {
        if (multiplierApplied)
        {
            betCalculator.ModifyGeneralModifier(-stackChipData.BonusAmount);
            multiplierApplied = false;
        }
    }
}