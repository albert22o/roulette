using UnityEngine;

public class BlackDozenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int bonusAmount;

    private bool modifyRequest = false;
    private DozenType selectedDozen;

    public BlackDozenChip(BetCalculator betCalculator, BallResults ballResults, BlackDozenChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyDozenMultiplier(selectedDozen, bonusAmount);
        }
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyDozenMultiplier(selectedDozen, -bonusAmount);
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        selectedDozen = GetRandomDozen();

        if (IsNumberInDozen(hit.FieldNumber, selectedDozen))
            modifyRequest = true;
        else
            modifyRequest = false;
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