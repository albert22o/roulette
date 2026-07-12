public class BaseRewardChangeChip : Chip
{
    private readonly RewardCalculator rewardCalculator;
    private readonly BaseRewardChangeChipData baseRewardChangeChipData;

    public BaseRewardChangeChip(RewardCalculator rewardCalculator, BaseRewardChangeChipData baseRewardChangeChipData) : base(baseRewardChangeChipData)
    {
        this.rewardCalculator = rewardCalculator;
        this.baseRewardChangeChipData = baseRewardChangeChipData;
    }

    public override void Apply()
    {
    }

    public override void Revert()
    {
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        rewardCalculator.BaseReward -= baseRewardChangeChipData.BaseRewardBonus;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        rewardCalculator.BaseReward += baseRewardChangeChipData.BaseRewardBonus;
    }
}