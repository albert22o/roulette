using System.Linq;
using UnityEngine;

public class ParityBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly ParityType parity;
    private readonly int bonusAmount;

    public ParityBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        ParityBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.parity = data.Parity;
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
        foreach (var hit in hits.Where(CheckParity))
        {
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
        }
    }

    private bool CheckParity(HitInfo hit)
    {
        switch (parity)
        {
            case ParityType.Even:
                return hit.FieldNumber % 2 == 0 && hit.FieldNumber != 0; 
            case ParityType.Odd:
                return hit.FieldNumber % 2 != 0;
            default:
                return false;
        }
    }
}