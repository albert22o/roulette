using UnityEngine;

public class AntiColorChip : Chip
{
    private readonly BallResults ballResults;
    private readonly ColorType targetColor;

    private bool rerollAdded = false;

    public AntiColorChip(BallResults ballResults, ColorType targetColor, AntiColorChipData data) : base(data)
    {
        this.ballResults = ballResults;
        this.targetColor = targetColor;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
    }

    public override void Revert()
    {
        rerollAdded = false;
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (rerollAdded)
            return;

        if (hit?.FieldColor == targetColor)
        {
            rerollAdded = true;
        }
    }
}