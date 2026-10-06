
using UnityEngine;

[CreateAssetMenu(fileName = "GoldenIngotChip", menuName = "Chips/GoldenIngotChip")]
public class GoldenIngotChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new GoldenIngotChip(deps.BetCalculator, deps.GoldenChips, deps.BallResults, deps.BetTable, this);
    }
}
