using UnityEngine;

public class ResourceModifierChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ResourceType resource;
    private readonly int amount;

    public ResourceModifierChip(
        BetCalculator betCalculator,
        ResourceModifierChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.resource = data.Resource;
        this.amount = data.Amount;
    }

    public override void Apply()
    {
        ApplyResource(amount);
    }

    public override void Revert()
    {
        ApplyResource(-amount);
    }

    private void ApplyResource(int value)
    {
        switch (resource)
        {
            case ResourceType.StartMoney:
                // TODO: Реализовать, когда будет доступ к TableConfig
                // if (value > 0) tableConfig.AddToStartSum(value);
                // else tableConfig.RemoveFromStartSum(Mathf.Abs(value));
                Debug.Log($"[ResourceModifierChip] TODO: изменить базовые деньги на {value}");
                break;

            case ResourceType.Reroll:
                // TODO: Реализовать, когда в RoundManager появится метод AddRerollCount
                // roundManager.AddRerollCount(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить рероллы на {value}");
                break;

            case ResourceType.Bet:
                // TODO: Реализовать, когда в RoundManager появится метод AddBetCount
                // roundManager.AddBetCount(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить ставки на {value}");
                break;

            case ResourceType.ChipSlot:
                // TODO: Реализовать, когда в ChipHolder появится метод ChangeCapacity
                // chipHolder.ChangeCapacity(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить слоты фишек на {value}");
                break;

            case ResourceType.BallSlot:
                // TODO: Реализовать, когда будет готов BallHolder с методом ChangeCapacity
                // ballHolder.ChangeCapacity(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить слоты шариков на {value}");
                break;

            case ResourceType.GoldenChip:
                // TODO: Реализовать, когда будет доступ к GoldenChips
                // if (value > 0) goldenChips.Add(value);
                // else goldenChips.Remove(Mathf.Abs(value));
                Debug.Log($"[ResourceModifierChip] TODO: изменить золотые фишки на {value}");
                break;

            case ResourceType.QuotaPercent:
                // TODO: Реализовать, когда в RoundManager появится метод ModifyGoal
                // roundManager.ModifyGoal(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить квоту на {value}%");
                break;

            case ResourceType.Reward:
                // TODO: Реализовать, когда в RoundManager появится метод ModifyReward
                // roundManager.ModifyReward(value);
                Debug.Log($"[ResourceModifierChip] TODO: изменить награду за квоту на {value}");
                break;
        }
    }
}