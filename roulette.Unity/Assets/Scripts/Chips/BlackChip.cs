using System.Linq;
using UnityEngine;

public class BlackChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    public BlackChip(BetCalculator betCalculator, RouletteBallsManager rouletteBallsManager, BlackChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
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

        foreach (var hit in hits.Where(h => h.FieldColor == ColorType.Black))
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, 1);
    }
}
