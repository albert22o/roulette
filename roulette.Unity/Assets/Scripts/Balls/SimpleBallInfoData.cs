using UnityEngine;

[CreateAssetMenu(fileName = "SimpleBallInfoData", menuName = "Balls/SimpleBallInfoData")]
public class SimpleBallInfoData : BallInfoData
{
    public override BallInfo CreateBall(string hash, BallDependencies deps)
    {
        return new SimpleBall(hash, this, deps);
    }
}