using UnityEngine;

[CreateAssetMenu(fileName = "InvestmentChip", menuName = "Chips/InvestmentChip")]
public class InvestmentChipData : ChipData
{
    public int BonusAmount = 200;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new InvestmentChip(deps.BetTable, this);
    }
}