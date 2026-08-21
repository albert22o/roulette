using UnityEngine;

public class InvestmentChip : Chip
{
    private readonly BetTable betTable;
    private readonly int bonusAmount;

    public InvestmentChip(BetTable betTable, InvestmentChipData data) : base(data)
    {
        this.betTable = betTable;
        bonusAmount = data.BonusAmount;
    }

    public override bool Apply()
    {
        return true;
    }

    public override void Revert()
    {
    }

    public override void OnAddedToHolder()
    {
        betTable.AddAvailableSum(bonusAmount);
    }

    public override void OnRemovedFromHolder()
    {
        betTable.RemoveAvailableSum(bonusAmount);
    }
}