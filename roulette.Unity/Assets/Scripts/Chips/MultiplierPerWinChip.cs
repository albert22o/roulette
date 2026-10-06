public class MultiplierPerWinChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RewardCalculator rewardCalculator;
    private readonly BetTable betTable;
    private readonly MultiplierPerWinChipData multiplierPerWinChipData;
    private float multiplier = 1f;
    private float appliedMultiplier;
    private bool wasApplied;
    private HitInfo ballHit;

    public override float AppliedValue => appliedMultiplier;
    public override OperationType Operation => OperationType.Multiply;

    public MultiplierPerWinChip(BetCalculator betCalculator,
        RewardCalculator rewardCalculator, BallResults ballResults, BetTable betTable,
        MultiplierPerWinChipData multiplierPerWinChipData) : base(multiplierPerWinChipData)
    {
        this.betCalculator = betCalculator;
        this.rewardCalculator = rewardCalculator;
        this.betTable = betTable;
        this.multiplierPerWinChipData = multiplierPerWinChipData;
        ballResults.BallSelected += OnBallSelected;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        rewardCalculator.GoalBeaten += OnGoalBeaten;
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        rewardCalculator.GoalBeaten -= OnGoalBeaten;
    }

    private void OnGoalBeaten(int x)
    {
        if (x < 2)
            return;
        multiplier += multiplierPerWinChipData.MultiplierPerWin;
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

        appliedMultiplier = multiplier;
        betCalculator.MultiplyGeneralModifier(appliedMultiplier);
        wasApplied = true;
        return true;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.DivideGeneralModifier(appliedMultiplier);
            wasApplied = false;
        }
    }
}