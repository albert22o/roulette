using UnityEngine;

public class ColorBankChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly ColorType targetColor;

    private int currentBonus = 0;
    private bool bonusApplied = false;
    private bool applyConditionsMatched;
    private int bonusDelta;

    public override OperationType Operation => OperationType.Add;

    public ColorBankChip(
        BetCalculator betCalculator,
        BallResults ballResults,
        BetTable betTable,
        ColorType targetColor,
        ColorBankChipData data,
        int bonusDelta) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        this.targetColor = targetColor;

        this.ballResults.BallSelected += OnBallSelected;
        this.bonusDelta = bonusDelta;
    }

    public override float AppliedValue => currentBonus;

    public override bool Apply()
    {
        if (!applyConditionsMatched) return false;
        if (!bonusApplied && currentBonus > 0)
        {
            betCalculator.ModifyGeneralModifier(currentBonus);
            bonusApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (bonusApplied && currentBonus > 0)
        {
            betCalculator.ModifyGeneralModifier(-currentBonus);
            bonusApplied = false;
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (hit?.FieldColor == targetColor)
        {
            currentBonus += bonusDelta;
        }

        applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit);
    }
}
