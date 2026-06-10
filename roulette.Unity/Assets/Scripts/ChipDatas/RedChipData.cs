using UnityEngine;
[CreateAssetMenu(fileName = "RedChipData", menuName = "Chips/RedChipData")]

public class RedChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RedChip(deps.BetCalculator, deps.RouletteBallsManager,  this);
    }
}
