using UnityEngine;

public class BlackDozenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int bonusAmount;
    private readonly BetTable betTable;

    private DozenType selectedDozen;

    private bool applyConditionsMatched;
    private bool multiplierApplied = false;

    public override int AppliedMultiplier => bonusAmount;

    public BlackDozenChip(BetCalculator betCalculator, BallResults ballResults,
        BlackDozenChipData data, BetTable betTable) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
        this.betTable = betTable;
    }

    public override bool Apply()
    {
        if (applyConditionsMatched)
        {
            betCalculator.ModifyDozenMultiplier(selectedDozen, bonusAmount);
            multiplierApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (multiplierApplied)
        {
            betCalculator.ModifyDozenMultiplier(selectedDozen, -bonusAmount);
            multiplierApplied = false;
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (hit == null)
        {
            applyConditionsMatched = false;
            return;
        }

        selectedDozen = GetRandomDozen();


        if (IsNumberInDozen(hit.FieldNumber, selectedDozen))
        {
            applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
                BetTableExtentions.CheckDozenMatch(betTable, hit) ||
                BetTableExtentions.CheckNumberMatch(betTable, hit);
        }
        else
        {
            applyConditionsMatched = false;
        }
    }

    private DozenType GetRandomDozen()
    {
        int random = UnityEngine.Random.Range(0, 3);
        return random switch
        {
            0 => DozenType.First,
            1 => DozenType.Second,
            2 => DozenType.Third,
            _ => DozenType.First
        };
    }

    private bool IsNumberInDozen(int number, DozenType dozen)
    {
        if (number == 0) return false;

        return dozen switch
        {
            DozenType.First => number >= 1 && number <= 12,
            DozenType.Second => number >= 13 && number <= 24,
            DozenType.Third => number >= 25 && number <= 36,
            _ => false
        };
    }
}