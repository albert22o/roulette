using UnityEngine;

public class InvestmentBonusChip : Chip
{
    private readonly Money money;
    private readonly int bonusAmount;

    private bool modifierApplied = false;

    public InvestmentBonusChip(
        Money money,
        InvestmentBonusChipData data) : base(data)
    {
        this.money = money;
        bonusAmount = data.BonusAmount;
    }

    public override void Apply()
    {
        money.Add(bonusAmount);
        modifierApplied = true;
    }

    public override void Revert()
    {
        if (modifierApplied)
        {
            money.Remove(bonusAmount);
            modifierApplied = false;
        }
    }
}