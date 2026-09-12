public class EmptyBallSlotBonusChip : Chip
{
    private readonly BallHolder ballHolder;
    private readonly BetCalculator betCalculator;
    private readonly EmptyBallSlotBonusChipData emptyBallSlotBonusChipData;
    private int bonus;

    public EmptyBallSlotBonusChip(BallHolder ballHolder, BetCalculator betCalculator,
        EmptyBallSlotBonusChipData emptyBallSlotBonusChipData) : base(emptyBallSlotBonusChipData)
    {
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.emptyBallSlotBonusChipData = emptyBallSlotBonusChipData;
    }

    public override int AppliedMultiplier => bonus;

    public override bool Apply()
    {
        bonus = ballHolder.FreeSlotsCount * emptyBallSlotBonusChipData.BonusPerBall;
        if (bonus == 0)
            return false;

        betCalculator.ModifyGeneralModifier(bonus);
        return true;
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-ballHolder.FreeSlotsCount * emptyBallSlotBonusChipData.BonusPerBall);
    }
}