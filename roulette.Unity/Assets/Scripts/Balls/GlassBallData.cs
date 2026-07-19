using UnityEngine;

[CreateAssetMenu(fileName = "GlassBallData", menuName = "Balls/GlassBallData")]
public class GlassBallData : BallInfoData
{
    [SerializeField][Range(0, 100)] private int destroyChance;
    [SerializeField] private int multiplier;

    public override BallInfo CreateBall(string hash, BallDependencies deps)
    {
        return new GlassBall(multiplier, destroyChance, hash, this, deps);
    }
}
