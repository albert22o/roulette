using System.Linq;
using UnityEngine;


public class AntiRedChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly RoundStateManager roundStateManager;
    private bool hasTriggered = false;

    public AntiRedChip(BetCalculator betCalculator, RouletteBallsManager rouletteBallsManager, RoundStateManager roundStateManager, AntiRedChipData data): base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.roundStateManager = roundStateManager;
    }

    public override void Apply()
    {
        rouletteBallsManager.AllBallsStopped += OnAllBallsStopped;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public override void Revert()
    {
        rouletteBallsManager.AllBallsStopped -= OnAllBallsStopped;
        roundStateManager.StateChanged -= OnStateChanged;
        hasTriggered = false;
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        if (newState == RoundStateManager.RoundState.PlacingBets)
            hasTriggered = false;
    }
    private void OnAllBallsStopped(HitInfo[] hits)
    {
        if (hasTriggered) return;

        var redBallHit = hits.FirstOrDefault(h => h.FieldColor == ColorType.Red);
        if (redBallHit != null)
        {
            hasTriggered = true;
            rouletteBallsManager.Clear(redBallHit.BallHash);
        }
    }

}
