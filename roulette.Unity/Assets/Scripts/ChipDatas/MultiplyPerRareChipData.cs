using UnityEngine;

[CreateAssetMenu(fileName = "MultiplyPerRareChip", menuName = "Chips/MultiplyPerRareChip")]
public class MultiplyPerRareChipData : ChipData
{
    public float MultiplierPerChip = 1.5f;
    public RarityType RarityTypeEffect;
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new MultiplyPerRareChip(deps.BetCalculator, deps.ChipHolder, deps.BallResults, deps.BetTable, this);
    }
}
