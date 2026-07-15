using UnityEngine;

[CreateAssetMenu(fileName = "EmptyBallSlotBonusChip", menuName = "Chips/EmptyBallSlotBonusChip")]
public class EmptyBallSlotBonusChipData : ChipData
{
    public int BonusPerBall;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new EmptyBallSlotBonusChip(deps.BallHolder, deps.BetCalculator, this);
    }
}
