using UnityEngine;

[CreateAssetMenu(fileName = "BaseRewardChangeChip", menuName = "Chips/BaseRewardChangeChip")]
public class BaseRewardChangeChipData : ChipData
{
    public int BaseRewardBonus = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new BaseRewardChangeChip(deps.RewardCalculator, this);
    }
}
