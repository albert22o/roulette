public class EmptyBallSlotBonusChip : Chip
{
    private readonly BallHolder ballHolder;
    private readonly BetCalculator betCalculator;
    private readonly BetTable betTable;
    private readonly EmptyBallSlotBonusChipData emptyBallSlotBonusChipData;
    private int bonus;
    private bool wasApplied;
    private HitInfo ballHit;

    public EmptyBallSlotBonusChip(BallHolder ballHolder, BetCalculator betCalculator,
        BallResults ballResults, BetTable betTable, EmptyBallSlotBonusChipData emptyBallSlotBonusChipData) : base(emptyBallSlotBonusChipData)
    {
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.betTable = betTable;
        this.emptyBallSlotBonusChipData = emptyBallSlotBonusChipData;

        ballResults.BallSelected += OnBallSelected;
    }

    public override float AppliedValue => bonus;

    public override OperationType Operation => OperationType.Add;

    private void OnBallSelected(HitInfo hit)
    {
        ballHit = hit;
    }

    public override bool Apply()
    {
        bonus = ballHolder.FreeSlotsCount * emptyBallSlotBonusChipData.BonusPerBall;

        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (bonus == 0 || !betMatched)
            return false;

        betCalculator.ModifyGeneralModifier(bonus);
        wasApplied = true;
        return true;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyGeneralModifier(-bonus);
            wasApplied = false;
        }
    }
}