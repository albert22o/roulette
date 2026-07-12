public class EmptyBallSlotBonusChip : Chip
{
    private readonly BallHolder ballHolder;
    private readonly BetCalculator betCalculator;
    private readonly EmptyBallSlotBonusChipData emptyBallSlotBonusChipData;

    public EmptyBallSlotBonusChip(BallHolder ballHolder, BetCalculator betCalculator,
        EmptyBallSlotBonusChipData emptyBallSlotBonusChipData) : base(emptyBallSlotBonusChipData)
    {
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.emptyBallSlotBonusChipData = emptyBallSlotBonusChipData;
    }

    public override void Apply()
    {
        betCalculator.ModifyGeneralModifier(ballHolder.FreeSlotsCount * emptyBallSlotBonusChipData.BonusPerBall);
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-ballHolder.FreeSlotsCount * emptyBallSlotBonusChipData.BonusPerBall);
    }
}