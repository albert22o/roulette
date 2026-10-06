public class AllInChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly float multiplier;

    private bool applyConditionsMatched;
    private bool multiplierApplied = false;

    public override float AppliedValue => multiplier;
    public override OperationType Operation => OperationType.Multiply;

    public AllInChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, AllInChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        multiplier = data.Multiplier;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override bool Apply()
    {
        if (applyConditionsMatched)
        {
            betCalculator.MultiplyGeneralModifier(multiplier);
            multiplierApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (multiplierApplied)
        {
            betCalculator.DivideGeneralModifier(multiplier);
            multiplierApplied = false;
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (IsBetOnlyOnZero())
        {
            applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
                BetTableExtentions.CheckDozenMatch(betTable, hit) ||
                BetTableExtentions.CheckNumberMatch(betTable, hit);
        }
        else
        {
            applyConditionsMatched = false;
        }
    }

    private bool IsBetOnlyOnZero()
    {
        int betOnZero = 0;
        int totalBets = 0;

        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            totalBets += amount;

            if (betInfo.FieldNumber == 0)
            {
                betOnZero += amount;
            }
        }
        return betOnZero > 0 && totalBets == betOnZero;
    }
}