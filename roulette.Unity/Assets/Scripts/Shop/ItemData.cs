using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public RarityType RarityType => rarityType;
    public string Name => itemName;
    public string Description => description;
    public GameObject ShopPrefab => shopPrefab;

    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private RarityType rarityType;
}
