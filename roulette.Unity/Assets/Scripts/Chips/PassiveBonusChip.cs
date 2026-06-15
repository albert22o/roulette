using UnityEngine;

public class PassiveBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int bonusAmount;
    private readonly SecondaryEffectType secondaryEffect;
    private readonly int secondaryAmount;

    public PassiveBonusChip(
        BetCalculator betCalculator,
        PassiveBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.bonusAmount = data.BonusAmount;
        this.secondaryEffect = data.SecondaryEffect;
        this.secondaryAmount = data.SecondaryAmount;
    }

    public override void Apply()
    {
        for (int i = 0; i <= 36; i++)
        {
            betCalculator.ModifyFieldNumberMultiplier(i, bonusAmount);
        }

        ApplySecondaryEffect(secondaryAmount);
    }

    public override void Revert()
    {
        for (int i = 0; i <= 36; i++)
        {
            betCalculator.ModifyFieldNumberMultiplier(i, -bonusAmount);
        }

        ApplySecondaryEffect(-secondaryAmount);
    }

    private void ApplySecondaryEffect(int value)
    {
        switch (secondaryEffect)
        {
            case SecondaryEffectType.None:
                // Ничего не делаем
                break;

            case SecondaryEffectType.RemoveBallSlot:
                // TODO: Реализовать, когда будет готов BallHolder
                // ballHolder.ChangeCapacity(-value);
                Debug.Log($"[PassiveBonusChip] TODO: изменить слоты шариков на {-value}");
                break;

            case SecondaryEffectType.RemoveBet:
                // TODO: Реализовать, когда в RoundManager появится метод AddBetCount
                // roundManager.AddBetCount(-value);
                Debug.Log($"[PassiveBonusChip] TODO: изменить ставки на {-value}");
                break;

            case SecondaryEffectType.RandomBallSelection:
                // TODO: Реализовать систему случайного выбора шариков
                // Это должна быть отдельная система, которая перехватывает выбор игрока
                // и вместо него делает случайный выбор
                Debug.Log($"[PassiveBonusChip] TODO: включить случайный выбор шариков");
                break;
        }
    }
}