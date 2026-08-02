using UnityEngine;

public class ItemReceiver
{
    private readonly ChipHolder chipHolder;
    private readonly BossChips bossChips;
    private readonly GoldenChips goldenChips;
    private readonly BallHolder ballHolder;
    private readonly CardHolder cardHolder;
    private readonly FoodHolder foodHolder;

    public ItemReceiver(ChipHolder chipHolder, BossChips bossChips, GoldenChips goldenChips,
        BallHolder ballHolder, CardHolder cardHolder, FoodHolder foodHolder)
    {
        this.chipHolder = chipHolder;
        this.bossChips = bossChips;
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.cardHolder = cardHolder;
        this.foodHolder = foodHolder;
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
            case CardData cardData:
                cardHolder.AddCard(cardData);
                break;
            case FoodData food:
                foodHolder.AddFood(food);
                break;
            default: 
                Debug.LogWarning($"Unknown type: {item?.GetType().Name}");
                break;
        }
    }
}
