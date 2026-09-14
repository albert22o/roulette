using UnityEngine;

public class AntiColorChip : Chip
{
    private readonly ColorType targetColor;
    private readonly IRouletteWheel rouletteWheel;
    private bool rerollAbility = true;

    public AntiColorChip(IRouletteWheel rouletteWheel, ColorType targetColor, AntiColorChipData data) : base(data)
    {
        this.targetColor = targetColor;
        this.rouletteWheel = rouletteWheel;
        rouletteWheel.AllBallsStopped += OnAllBallsStopped;
    }

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

    public override bool Apply()
    {
        return false;
    }

    public override void Revert()
    {
        rerollAbility = true;
    }

    private void OnAllBallsStopped(HitInfo[] hit)
    {
        if (!rerollAbility)
            return;
        rerollAbility = false;
        foreach (var hitInfo in hit)
        {
            if (hitInfo.FieldColor == targetColor)
            {
                rouletteWheel.AddBallsToAutoReroll(hitInfo.BallHash);
                return;
            }
        }
    }
}