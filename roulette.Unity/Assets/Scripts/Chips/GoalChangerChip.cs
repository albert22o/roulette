public class GoalChangerChip : Chip
{
    private readonly GoalChangerChipData percentChipData;
    private readonly RoundManager roundManager;

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

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
        return false;
    }

    public override void Revert() { }
}