using UnityEngine;

public enum ParityType
{
    Even,  
    Odd    
}

[CreateAssetMenu(fileName = "ParityBonusChipData", menuName = "Chips/ParityBonusChipData")]
public class ParityBonusChipData : ChipData
{
    [Header("Settings")]
    [Tooltip("Тип четности")]
    public ParityType Parity;

    [Tooltip("Сколько добавить к множителю")]
    public int BonusAmount = 1;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ParityBonusChip(deps.BetCalculator, deps.RouletteBallsManager, this);
    }
}