public class ChangeBallCountChip : Chip
{
    private BallHolder ballHolder;
    private BetCalculator betCalculator;
    private ChangeBallCountChipData changeBallCountChipData;

    public ChangeBallCountChip(BallHolder ballHolder, BetCalculator betCalculator,
        ChangeBallCountChipData changeBallCountChipData) : base(changeBallCountChipData)
    {
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.changeBallCountChipData = changeBallCountChipData;
    }

    public override bool Apply() 
    {
        betCalculator.ModifyGeneralModifier(changeBallCountChipData.ModifyAmount);
        return true;
    }

    public override void Revert() 
    {
        betCalculator.ModifyGeneralModifier(-changeBallCountChipData.ModifyAmount);
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        ballHolder.ChangeCapacity(-changeBallCountChipData.BallSlotToRemove);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        ballHolder.ChangeCapacity(changeBallCountChipData.BallSlotToRemove);
    }
}