using UnityEngine;

[CreateAssetMenu(fileName = "AntiRedChipData", menuName ="Chips/AntiRedChipData")]
public class AntiRedChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new AntiRedChip(deps.BetCalculator,deps.RouletteBallsManager,deps.RoundStateManager, this);
    }
}
