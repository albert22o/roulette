using UnityEngine;

[CreateAssetMenu(fileName = "ColorBankChip", menuName = "Chips/ColorBankChip")]
public class ColorBankChipData : ChipData
{
    [SerializeField] private ColorType targetColor;
    [SerializeField] private int bonusDelta = 2;
    public ColorType TargetColor => targetColor;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ColorBankChip(deps.BetCalculator, deps.BallResults, deps.BetTable, targetColor, this, bonusDelta);
    }
}