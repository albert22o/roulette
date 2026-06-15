using UnityEngine;

public enum ParityType
{
    Even,  
    Odd    
}

[CreateAssetMenu(fileName = "ParityBonusChipData", menuName = "Chips/ParityBonusChipData")]
public class ParityBonusChipData : ChipData
{
    [Header ("Настройки")]
    public ParityType Parity;

    public int BonusAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ParityBonusChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}