using UnityEngine;

[CreateAssetMenu(fileName = "BallHolderConfig", menuName = "Scriptable Objects/BallHolderConfig")]
public class BallHolderConfig : ScriptableObject
{
    [field: SerializeField] public int Capacity { get; private set; }
    [field: SerializeField] public BallInfoData[] StartBalls { get; private set; }
}
