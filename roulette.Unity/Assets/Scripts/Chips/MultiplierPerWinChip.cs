public class MultiplierPerWinChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RewardCalculator rewardCalculator;
    private readonly MultiplierPerWinChipData multiplierPerWinChipData;
    private float multiplier = 1f;

    public MultiplierPerWinChip(BetCalculator betCalculator, 
        RewardCalculator rewardCalculator, MultiplierPerWinChipData multiplierPerWinChipData) : base(multiplierPerWinChipData)
    {
        this.betCalculator = betCalculator;
        this.rewardCalculator = rewardCalculator;
        this.multiplierPerWinChipData = multiplierPerWinChipData;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        rewardCalculator.GoalBeaten += OnGoalBeaten;
    }


    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        rewardCalculator.GoalBeaten -= OnGoalBeaten;
    }

    private void OnGoalBeaten(float x)
    {
        if (x < 2)
            return;
        multiplier += multiplierPerWinChipData.MultiplierPerWin;
    }

    public override bool Apply()
    {
        betCalculator.MuliplyGeneralModifier(multiplier);
        return true;
    }

    public override void Revert()
    {
        betCalculator.DivideGeneralModifier(multiplier);
    }
}