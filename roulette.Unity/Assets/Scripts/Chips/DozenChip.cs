using System;

public class DozenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly DozenType dozenType;
    private readonly int modifyAmount;
    private HitInfo ballHit;

    public override int AppliedMultiplier => modifyAmount;

    public DozenChip(BetCalculator betCalculator, DozenChipData dozenChipData, BallResults ballResults) : base(dozenChipData)
    {
        this.betCalculator = betCalculator;
        dozenType = dozenChipData.DozenType;
        modifyAmount = dozenChipData.ModifyAmount;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo info)
    {
        ballHit = info;
    }

    public override bool Apply()
    {
        betCalculator.ModifyDozenMultiplier(dozenType, modifyAmount);
        return ballHit != null && ballHit.Dozen == dozenType;
    }

    public override void Revert()
    {
        betCalculator.ModifyDozenMultiplier(dozenType, -modifyAmount);
    }
}
