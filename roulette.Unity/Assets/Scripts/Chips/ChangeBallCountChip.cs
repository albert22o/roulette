public class ChangeBallCountChip : Chip
{
    private BallHolder ballHolder;
    private BetCalculator betCalculator;
    private ChangeBallCountChipData changeBallCountChipData;
    private readonly BetTable betTable;
    private bool applyConditionsMatched;
    private bool multiplierApplied = false;

    public override float AppliedValue => changeBallCountChipData.ModifyAmount;
    public override OperationType Operation => OperationType.Add;

    public ChangeBallCountChip(BallHolder ballHolder, BetCalculator betCalculator,
        ChangeBallCountChipData changeBallCountChipData, BetTable betTable, BallResults ballResults) : base(changeBallCountChipData)
    {
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.changeBallCountChipData = changeBallCountChipData;
        this.betTable = betTable;

        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit);
    }

    public override bool Apply()
    {
        if (applyConditionsMatched)
        {
            betCalculator.ModifyGeneralModifier(changeBallCountChipData.ModifyAmount);
            multiplierApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (multiplierApplied)
        {
            betCalculator.ModifyGeneralModifier(-changeBallCountChipData.ModifyAmount);
            multiplierApplied = false;
        }
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        ballHolder.ChangeCapacity(-changeBallCountChipData.BallSlotToRemove);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        ballHolder.ChangeCapacity(changeBallCountChipData.BallSlotToRemove);
    }
}