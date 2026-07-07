using UnityEngine;

public class AllInChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly float multiplier;

    private bool modifyRequest = false;

    public AllInChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable,AllInChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        multiplier = data.Multiplier; 

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
        if (modifyRequest)
            betCalculator.MuliplyGeneralModifier(multiplier);
    }

    public override void Revert()
    {
        if (modifyRequest)
            betCalculator.DivideGeneralModifier(multiplier);
    }

    private void OnBallSelected(HitInfo _)
    {
        if (IsBetOnlyOnZero())
        {
            modifyRequest = true;
        }
        else
        {
            modifyRequest = false;
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