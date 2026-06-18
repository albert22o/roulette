using UnityEngine;

[CreateAssetMenu(fileName = "DivisibilityBonusChip", menuName = "Chips/DivisibilityBonusChip")]
public class DivisibilityBonusChipData : ChipData
{
    public int Divisior;
    public int Remainder = 0;
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DivisibilityBonusChip(
            deps.BetCalculator,
            deps.RouletteBallsManager,
            deps.OnBallSelected,
            this);
    }
}