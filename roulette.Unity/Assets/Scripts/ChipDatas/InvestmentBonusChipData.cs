using UnityEngine;

[CreateAssetMenu(fileName = "InvestmentBonusChip", menuName = "Chips/InvestmentBonusChip")]
public class InvestmentBonusChipData : ChipData
{
    public int BonusAmount = 200;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new InvestmentBonusChip(deps.Money, this);
    }
}