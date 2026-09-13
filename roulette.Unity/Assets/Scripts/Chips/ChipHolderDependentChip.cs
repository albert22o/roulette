using UnityEngine;

// Базовый класс для фишек, которые считают что-то в ChipHolder
public abstract class ChipHolderDependentChip : Chip
{
    protected readonly BetCalculator betCalculator;
    protected readonly ChipHolder chipHolder;
    protected readonly BetTable betTable;
    protected readonly BallResults ballResults;
    private bool applyConditionsMatched;
    private bool bonusApplied = false;
    protected int Bonus;

    protected ChipHolderDependentChip(BetCalculator betCalculator, ChipHolder chipHolder, ChipData data,
        BallResults ballResults, BetTable betTable) : base(data)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.betTable = betTable;
        this.ballResults = ballResults;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit);
    }

    protected abstract int CalculateBonus();

    public override bool Apply()
    {
        if (applyConditionsMatched == false) return false;
        if (bonusApplied) return false;
        Bonus = CalculateBonus();
        if (Bonus > 0)
        {
            betCalculator.ModifyGeneralModifier(Bonus);
            bonusApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (!bonusApplied) return;
        betCalculator.ModifyGeneralModifier(-Bonus);
        bonusApplied = false;
    }
}