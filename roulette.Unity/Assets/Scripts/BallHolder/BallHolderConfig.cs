using UnityEngine;

[CreateAssetMenu(fileName = "BallHolderConfig", menuName = "Configs/BallHolderConfig")]
public class BallHolderConfig : ScriptableObject
{
    [SerializeField] public int Capacity { get; private set; }
    [SerializeField] public BallInfoData[] StartBalls { get; private set; }
}
