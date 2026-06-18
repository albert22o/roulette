using UnityEngine;

[CreateAssetMenu(fileName = "PrimeBonusChip", menuName = "Chips/PrimeBonusChip")]
public class PrimeBonusChipData : ChipData
{
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PrimeBonusChip(
            deps.BetCalculator,
            deps.RouletteBallsManager,
            deps.OnBallSelected,
            this);
    }
}