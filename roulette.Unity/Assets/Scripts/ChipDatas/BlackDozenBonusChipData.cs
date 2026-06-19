using UnityEngine;

[CreateAssetMenu(fileName = "BlackDozenBonusChip", menuName = "Chips/BlackDozenBonusChip")]
public class BlackDozenBonusChipData : ChipData
{
    public int BonusAmount = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new BlackDozenBonusChip(deps.BetCalculator, deps.RouletteBallsManager, deps.OnBallSelected, this);
    }
}