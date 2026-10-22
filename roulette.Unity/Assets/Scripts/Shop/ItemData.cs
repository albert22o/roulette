using UnityEngine;
using UnityEngine.Localization;

public abstract class ItemData : ScriptableObject
{
    public RarityType RarityType => rarityType;
    public string Name => itemName.Resolve();
    public string Description => description.Resolve();
    public GameObject ShopPrefab => shopPrefab;

    [SerializeField] private LocalizedString itemName;
    [SerializeField] private LocalizedString description;
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private RarityType rarityType = RarityType.None;
    [SerializeField] public Sprite Icon;
}
