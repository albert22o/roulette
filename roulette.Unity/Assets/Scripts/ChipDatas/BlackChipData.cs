using UnityEngine;
[CreateAssetMenu(fileName = "RedChipData", menuName = "Chips/RedChipData")]

public class BlackChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new BlackChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}
