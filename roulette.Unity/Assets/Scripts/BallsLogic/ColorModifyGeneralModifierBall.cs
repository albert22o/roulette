using System.Diagnostics;
using UnityEditor.PackageManager;

public class ColorModifyGeneralModifierBall : BallInfo
{
    private ColorType color;
    private int multiplier;
    private bool isSelected;
    public ColorModifyGeneralModifierBall(ColorType color, int multiplier, string hash, BallInfoData data, BallDependencies deps) : base(hash, data, deps)
    {
        this.color = color;
        this.multiplier = multiplier;
    }

    public override void ApplySelectedEffect(HitInfo hitInfo)
    {
        if (hitInfo.FieldColor == color)
        {
            Dependencies.BetCalculator.ModifyGeneralModifier(multiplier);
            isSelected = true;
            UnityEngine.Debug.Log("generalModIncreased");
        }
    }

    public override void RemoveSelectedEffect()
    {
        if (isSelected)
        {
            Dependencies.BetCalculator.ModifyGeneralModifier(-multiplier);
            isSelected = false;

            UnityEngine.Debug.Log("generalModDecreased");
        }
    }
}
