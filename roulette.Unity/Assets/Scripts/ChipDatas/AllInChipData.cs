using UnityEngine;

[CreateAssetMenu(fileName = "AllInChip", menuName = "Chips/AllInChip")]
public class AllInChipData : ChipData
{
    public float Multiplier = 3f;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new AllInChip(deps.BetCalculator, deps.BallResults, deps.BetTable, this);
    }
}