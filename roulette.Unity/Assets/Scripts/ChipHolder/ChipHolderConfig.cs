using UnityEngine;

[CreateAssetMenu(fileName = "ChipHolderConfig", menuName = "Scriptable Objects/ChipHolderConfig")]
public class ChipHolderConfig : ScriptableObject
{
    [field : SerializeField] public int Capacity { get; private set; }
}
