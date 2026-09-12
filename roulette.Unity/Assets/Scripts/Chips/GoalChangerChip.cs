public class GoalChangerChip : Chip
{
    private readonly GoalChangerChipData percentChipData;
    private readonly RoundManager roundManager;

    public override int AppliedMultiplier => 0;

    public GoalChangerChip(RoundManager roundManager, GoalChangerChipData percentChipData) : base(percentChipData)
    { 
        this.percentChipData = percentChipData;
        this.roundManager = roundManager;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        roundManager.ModifiGoalModifier(percentChipData.GoalModifierDelta);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        roundManager.ModifiGoalModifier(-percentChipData.GoalModifierDelta);
    }

    public override bool Apply()
    {
        return true;
    }

    public override void Revert() { }
}