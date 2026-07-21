using UnityEngine;

[CreateAssetMenu(fileName = "ColorModifyGeneralModifierBall", menuName = "Balls/ColorModifyGeneralModifierBall")]
public class ColorModifyGeneralModifierBallData : BallInfoData
{
    [SerializeField] private int multiplier;
    [SerializeField] private ColorType color;

    public int Multiplier => multiplier;
    public ColorType Color => color;

    public override BallInfo CreateBall(string hash, BallDependencies deps)
    {
        return new ColorModifyGeneralModifierBall(hash, this, deps);
    }
}