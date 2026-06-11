using UnityEngine;

[CreateAssetMenu(fileName = "PassiveBonusChipData", menuName = "Chips/PassiveBonusChipData")]
public class PassiveBonusChipData : ChipData
{
    [Header("Settings")]
    [Tooltip("Сколько добавить к множителю всех чисел (0-36)")]
    public int BonusAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PassiveBonusChip(deps.BetCalculator, this);
    }
}