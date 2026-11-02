using UnityEditor;
using UnityEngine;

public class InvestmentChip : Chip
{
    private readonly int bonusAmount;
    private readonly TableInfo tableInfo;

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

    public InvestmentChip(TableInfo tableInfo, InvestmentChipData data) : base(data)
    {
        bonusAmount = data.BonusAmount;
        this.tableInfo = tableInfo;
    }

    public override bool Apply()
    {
        return false;
    }

    public override void Revert()
    {
    }

    public override void OnAddedToHolder()
    {
        tableInfo.AddToStartSum(bonusAmount);
    }

    public override void OnRemovedFromHolder()
    {
        tableInfo.RemoveFromStartSum(bonusAmount);
    }
}