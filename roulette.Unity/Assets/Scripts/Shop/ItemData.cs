using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public RarityType RarityType => rarityType;
    public string Name => itemName;
    public string Description => description;
    public GameObject ShopPrefab => shopPrefab;

    [SerializeField] [TextArea] private string itemName;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private RarityType rarityType = RarityType.None;
    [SerializeField] public Sprite Icon;
}
