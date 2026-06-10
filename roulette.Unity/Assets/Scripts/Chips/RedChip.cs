using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class RedChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    public RedChip(BetCalculator betCalculator, RouletteBallsManager rouletteBallsManager, RedChipData data) : base(data)
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

        foreach (var hit in hits.Where(h => h.FieldColor == ColorType.Red))
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, 1);
    }
}
