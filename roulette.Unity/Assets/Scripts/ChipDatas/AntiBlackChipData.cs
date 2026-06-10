using UnityEngine;

[CreateAssetMenu(fileName = "AntiBlackChipData", menuName = "Chips/AntiBlackChipData")]
public class AntiBlackChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new AntiBlackChip(deps.BetCalculator, deps.RouletteBallsManager, deps.RoundStateManager, this);
    }
}
