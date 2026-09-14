using System;

public class OneToThirtySixChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int[] pseudoZeroNumbers = { 1, 2, 35, 36 };
    private HitInfo ballHit;

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

    public OneToThirtySixChip(BetCalculator betCalculator, OneToThirtySixChipData data, BallResults results) : base(data)
    {
        this.betCalculator = betCalculator;
        results.BallSelected += OnBallSelect;
    }

    private void OnBallSelect(HitInfo info)
    {
        ballHit = info;
    }

    public override bool Apply()
    {
        foreach (int number in pseudoZeroNumbers)
            betCalculator.AddToZeroFields(number);
        return false;
        //return ballHit != null && System.Array.IndexOf(pseudoZeroNumbers, ballHit.FieldNumber) >= 0;
    }

    public override void Revert()
    {
        foreach (int number in pseudoZeroNumbers)
            betCalculator.RemoveFromZeroFields(number);
    }
}