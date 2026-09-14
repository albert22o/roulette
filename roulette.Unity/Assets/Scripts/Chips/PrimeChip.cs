using UnityEngine;

public class PrimeChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly int bonusAmount;

    private bool modifyRequest = false;
    private static readonly int[] primeNumbers = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };

    public override float AppliedValue => bonusAmount;
    public override OperationType Operation => OperationType.Add;

    public PrimeChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, PrimeChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override bool Apply()
    {
        if (!modifyRequest)
            return false;

        betCalculator.ModifyGeneralModifier(bonusAmount);
        return true;
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyGeneralModifier(-bonusAmount);
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        bool primeMatched = hit != null && IsPrime(hit.FieldNumber);

        bool betMatched = hit != null && (
            BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit));

        modifyRequest = primeMatched && betMatched;
    }

    private bool IsPrime(int number)
    {
        foreach (int prime in primeNumbers)
        {
            if (number == prime)
                return true;
        }
        return false;
    }
}