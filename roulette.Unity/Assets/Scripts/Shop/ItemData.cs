using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private GameObject shopPrefab;

    public string Name => itemName;
    public string Description => description;

    public GameObject ShopPrefab => shopPrefab;
}
