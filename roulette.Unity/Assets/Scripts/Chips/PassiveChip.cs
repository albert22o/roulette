public class PassiveChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BetTable betTable;
    private readonly int bonusAmount;
    private HitInfo ballHit;
    private bool wasApplied;
    public override OperationType Operation => OperationType.Add;
    public override float AppliedValue => bonusAmount;

    public PassiveChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, PassiveChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.betTable = betTable;
        bonusAmount = data.BonusAmount;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        ballHit = hit;
    }

    public override bool Apply()
    {
        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (!betMatched)
            return false;

        betCalculator.ModifyGeneralModifier(bonusAmount);
        wasApplied = true;
        return true;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyGeneralModifier(-bonusAmount);
            wasApplied = false;
        }
    }
}