using UnityEngine;

[CreateAssetMenu(fileName = "DivisibilityBonusChip", menuName = "Chips/DivisibilityBonusChip")]

public class DivisibilityBonusChipData : ChipData
{
    public int Divisior;
    public int BonusAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DivisibilityBonusChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}
