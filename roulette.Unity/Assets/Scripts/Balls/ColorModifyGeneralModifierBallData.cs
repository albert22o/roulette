using UnityEngine;

[CreateAssetMenu(fileName = "ColorModifyGeneralModifierBall", menuName = "Balls/ColorModifyGeneralModifierBall")]
public class ColorModifyGeneralModifierBallData : BallInfoData
{
    [SerializeField] private int multiplier;
    [SerializeField] private ColorType color;

    public override BallInfo CreateBall(string hash, BallDependencies deps)
    {
        return new ColorModifyGeneralModifierBall(color, multiplier, hash, this, deps);
    }
}