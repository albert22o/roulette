using UnityEngine;

[CreateAssetMenu(fileName = "BetCountChip", menuName = "Chips/BetCountChip")]
public class BetCountChipData : ChipData
{
    public int BetCountDelta;
    public int BonusAmount;
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new BetCountChip(deps.TableInfo, deps.BetCalculator, this);
    }
}
