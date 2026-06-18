using UnityEngine;

[CreateAssetMenu(fileName = "SimpleBallInfoData", menuName = "Balls/SimpleBallInfoData")]
public class SimpleBallInfoData : BallInfoData
{
    public override BallInfo CreateBall(BallDependencies deps)
    {
        return new SimpleBallInfo(this);
    }
}