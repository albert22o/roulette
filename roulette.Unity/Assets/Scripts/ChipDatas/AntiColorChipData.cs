using UnityEngine;

[CreateAssetMenu(fileName = "AntiColorChip", menuName = "Chips/AntiColorChip")]
public class AntiColorChipData : ChipData
{
    [SerializeField] private ColorType targetColor;

    public ColorType TargetColor => targetColor;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new AntiColorChip(deps.RouletteWheel,  targetColor, this);
    }
}
