using UnityEngine;

public class ItemReceiver
{
    private readonly ChipHolder chipHolder;
    private readonly BossChips bossChips;
    private readonly GoldenChips goldenChips;
    private readonly BallHolder ballHolder;
    private readonly BetCalculator betCalculator;
    private readonly CardCollection cardCollection;

    public ItemReceiver(ChipHolder chipHolder, BossChips bossChips, GoldenChips goldenChips, BallHolder ballHolder, BetCalculator betCalculator, CardCollection cardCollection)
    {
        this.chipHolder = chipHolder;
        this.bossChips = bossChips;
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.betCalculator = betCalculator;
        this.cardCollection = cardCollection;
    }

    public void ReceiveItem(ItemData item)
    {
        switch (item)
        {
            case ChipData chip: 
                chipHolder.AddChip(chip); 
                break;
            case BallInfoData ball: 
                ballHolder.AddBall(ball); 
                break;
            case GoldenChipItemData gold: 
                goldenChips.Add(gold.Amount); 
                break;
            case BossChipItemData boss: 
                bossChips.Add(boss.Amount); 
                break;
            case GeneralMultipilerCardData generalMultiplierCardData:
                betCalculator.ModifyGeneralModifier(generalMultiplierCardData.Multiplier);
                cardCollection.AddCard(generalMultiplierCardData);
                break;
            case FoodData food:
                Debug.Log($"Food: {food.Name}"); 
                break;
            default: 
                Debug.LogWarning($"Unknown type: {item?.GetType().Name}");
                break;
        }
    }
}
