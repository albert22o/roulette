using UnityEngine;

[CreateAssetMenu(fileName = "MultiplierPerWinChip", menuName = "Chips/MultiplierPerWinChip")]
public class MultiplierPerWinChipData : ChipData
{
    public float MultiplierPerWin = 0.25f;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new MultiplierPerWinChip(deps.BetCalculator, deps.RewardCalculator, this);
    }
}
