using UnityEngine;

[CreateAssetMenu(fileName = "GoalChangerChip", menuName = "Chips/GoalChangerChip")]
public class GoalChangerChipData : ChipData
{
    public float GoalModifierDelta;
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new GoalChangerChip(deps.RoundManager, this);
    }
}
