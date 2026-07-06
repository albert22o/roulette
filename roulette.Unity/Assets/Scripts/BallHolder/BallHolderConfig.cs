using UnityEngine;

[CreateAssetMenu(fileName = "BallHolderConfig", menuName = "Configs/BallHolderConfig")]
public class BallHolderConfig : ScriptableObject
{
    [SerializeField] public int capacity;
    [SerializeField] public BallInfoData[] startBalls;

    public int Capacity => capacity;
    public BallInfoData[] StartBalls  => startBalls;
}
