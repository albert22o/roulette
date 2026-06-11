using System.Linq;
using UnityEngine;

public class RangeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    public RangeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        RangeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.minNumber = data.MinNumber;
        this.maxNumber = data.MaxNumber;
        this.bonusAmount = data.BonusAmount;
    }

    public override void Apply()
    {
        rouletteBallsManager.AllBallsStopped += OnAllBallsStopped;
    }

    public override void Revert()
    {
        rouletteBallsManager.AllBallsStopped -= OnAllBallsStopped;
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        foreach (var hit in hits.Where(CheckRange))
        {
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
        }
    }

    private bool CheckRange(HitInfo hit)
    {
        return hit.FieldNumber >= minNumber && hit.FieldNumber <= maxNumber;
    }
}