public class D6Chip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly D6ChipData d6ChipData;

    public D6Chip(BetCalculator betCalculator, BallResults ballResults, D6ChipData d6ChipData) : base(d6ChipData)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.d6ChipData = d6ChipData;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        ballResults.RandomBallSelection = true;
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        ballResults.RandomBallSelection = false;
    }

    public override void Apply()
    {
        betCalculator.ModifyGeneralModifier(d6ChipData.ModifyAmount);
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-d6ChipData.ModifyAmount);
    }
}