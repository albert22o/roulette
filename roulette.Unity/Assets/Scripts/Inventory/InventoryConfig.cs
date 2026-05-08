using UnityEngine;

[CreateAssetMenu(fileName = "InventoryConfig", menuName = "Scriptable Objects/InventoryConfig")]
public class InventoryConfig : ScriptableObject
{
    [Range(0,50)]
    [field: SerializeField] public int ChipsSlots { get; private set; }
    [Range(0, 50)]
    [field: SerializeField] public int BallsSlots { get; private set; }
}
