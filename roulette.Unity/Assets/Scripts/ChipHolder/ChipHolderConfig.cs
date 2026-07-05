using UnityEngine;

[CreateAssetMenu(fileName = "ChipHolderConfig", menuName = "Configs/ChipHolderConfig")]
public class ChipHolderConfig : ScriptableObject
{
    [field : SerializeField] public int Capacity { get; private set; }
}
