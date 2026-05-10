using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public string NameKey { get; private set; }
    [field: SerializeField] public string DescriptionKey { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public ItemType Type { get; private set; }

}

public enum ItemType
{
    Ball,
    Chip
}
