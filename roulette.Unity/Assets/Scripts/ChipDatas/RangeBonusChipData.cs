using UnityEngine;

[CreateAssetMenu(fileName = "RangeBonusChip", menuName = "Chips/RangeBonusChip")]
public class RangeBonusChipData : ChipData
{
    public int MinNumber = 0;
    public int MaxNumber = 36;
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RangeBonusChip(deps.BetCalculator, deps.RouletteBallsManager, deps.OnBallSelected, this);
    }
}