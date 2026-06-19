using UnityEngine;

[CreateAssetMenu(fileName = "RandomNumbersBonusChip", menuName = "Chips/RandomNumbersBonusChip")]
public class RandomNumbersBonusChipData : ChipData
{
    public int BonusAmount = 4;
    public int NumbersCount = 4;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RandomNumbersBonusChip(deps.BetCalculator, deps.RouletteBallsManager, deps.OnBallSelected, this);
    }
}