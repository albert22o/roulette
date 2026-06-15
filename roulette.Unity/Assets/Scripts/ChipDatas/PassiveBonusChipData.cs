using UnityEngine;

// Типы вторичных эффектов, которые могут идти вместе с бонусом к множителю
public enum SecondaryEffectType
{
    None,                  // Нет вторичного эффекта
    RemoveBallSlot,        // -N слот для шарика (TODO: нужен BallHolder)
    RemoveBet,             // -N ставок (TODO: нужен метод в RoundManager)
    RandomBallSelection    // Шарики выбираются случайно (TODO: нужна система выбора)
}

[CreateAssetMenu(fileName = "PassiveBonusChipData", menuName = "Chips/PassiveBonusChipData")]
public class PassiveBonusChipData : ChipData
{
    public int BonusAmount = 1;

    // Тип вторичного эффекта (если есть)
    public SecondaryEffectType SecondaryEffect = SecondaryEffectType.None;
    
    // Сила вторичного эффекта (обычно 1)
    public int SecondaryAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PassiveBonusChip(deps.BetCalculator, this);
    }
}