using UnityEngine;

[CreateAssetMenu(fileName = "RangeBonusChipData", menuName = "Chips/RangeBonusChipData")]
public class RangeBonusChipData : ChipData
{
    [Header("Settings")]
    [Tooltip("Минимальное число диапазона (включительно)")]
    [Range(0, 36)]
    public int MinNumber = 1;

    [Tooltip("Максимальное число диапазона (включительно)")]
    [Range(0, 36)]
    public int MaxNumber = 36;

    [Tooltip("Сколько добавить к множителю")]
    public int BonusAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RangeBonusChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}