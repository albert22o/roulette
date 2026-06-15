using UnityEngine;

public enum ResourceType
{
    StartMoney,      // Базовые деньги (TODO: нужен TableConfig.AddToStartSum)
    Reroll,          // Рероллы (TODO: нужен RoundManager.AddRerollCount)
    Bet,             // Ставки (TODO: нужен RoundManager.AddBetCount)
    ChipSlot,        // Слоты для фишек (TODO: нужен ChipHolder.ChangeCapacity)
    BallSlot,        // Слоты для шариков (TODO: нужен BallHolder.ChangeCapacity)
    GoldenChip,      // Золотые фишки (TODO: нужен GoldenChips.Add/Remove)
    QuotaPercent,    // Процент квоты (TODO: нужен RoundManager.ModifyGoal)
    Reward           // Награда за квоту (TODO: нужен RoundManager.ModifyReward)
}

[CreateAssetMenu(fileName = "ResourceModifierChipData", menuName = "Chips/ResourceModifierChipData")]
public class ResourceModifierChipData : ChipData
{
    // Какой ресурс меняем
    public ResourceType Resource;

    // Сколько добавить
    public int Amount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ResourceModifierChip(deps.BetCalculator, this);
    }
}