public class MultiplyPerRareChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly BetTable betTable;
    private readonly MultiplyPerRareChipData multiplyPerRareChipData;
    private float multiplier = 1f;
    private float appliedMultiplier;
    private bool wasApplied;
    private HitInfo ballHit;
    public override OperationType Operation => OperationType.Multiply;
    public override float AppliedValue => appliedMultiplier;
    public MultiplyPerRareChip(BetCalculator betCalculator, ChipHolder chipHolder, BallResults ballResults, BetTable betTable, MultiplyPerRareChipData multiplyPerRareChipData) : base(multiplyPerRareChipData)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.betTable = betTable;
        this.multiplyPerRareChipData = multiplyPerRareChipData;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        ballHit = hit;
    }

    public override bool Apply()
    {
        multiplier = 1f;
        foreach (var chip in chipHolder.Chips)
        {
            if (chip == null) continue;
            if (chip.ChipData.RarityType == multiplyPerRareChipData.RarityTypeEffect)
                multiplier += multiplyPerRareChipData.MultiplierPerChip;
        }

        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (multiplier == 1f || !betMatched)
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