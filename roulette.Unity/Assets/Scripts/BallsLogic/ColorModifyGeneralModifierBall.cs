public class ColorModifyGeneralModifierBall : BallInfo
{
    private ColorType color;
    private int multiplier;
    private bool isSelected;
    public ColorModifyGeneralModifierBall(string hash, ColorModifyGeneralModifierBallData data, BallDependencies deps) : base(hash, data, deps)
    {
        color = data.Color;
        multiplier = data.Multiplier;
    }

    public override void ApplySelectedEffect(HitInfo hitInfo)
    {
        if (hitInfo.FieldColor == color)
        {
            Dependencies.BetCalculator.ModifyGeneralModifier(multiplier);
            isSelected = true;
        }
    }

    public override void RemoveSelectedEffect()
    {
        if (isSelected)
        {
            Dependencies.BetCalculator.ModifyGeneralModifier(-multiplier);
            isSelected = false;
        }
    }
}
