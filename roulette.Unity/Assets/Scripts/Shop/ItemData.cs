using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string nameKey;
    [SerializeField] private string descriptionKey;
    [SerializeField] private int cost;
    [SerializeField] private ItemType type;
    [Header("Display in shop")]
    [SerializeField] private ItemVisual prefab;
    [SerializeField] private VisualComponent visualComponent;


    public string NameKey => nameKey;
    public string DescriptionKey => descriptionKey;
    public int Cost => cost;
    public ItemType Type => type;
    public ItemVisual Prefab => prefab;
    public VisualComponent VisualComponent => visualComponent;
}

public enum ItemType
{
    Ball,
    Chip,
    Food,
}
