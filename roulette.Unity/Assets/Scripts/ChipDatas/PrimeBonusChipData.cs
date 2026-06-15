using UnityEngine;

[CreateAssetMenu(fileName = "PrimeBonusChipData", menuName = "Chips/PrimeBonusChipData")]
public class PrimeBonusChipData : ChipData
{
    public int BonusAmount = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PrimeBonusChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}