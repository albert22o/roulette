using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsRarityGroup", menuName = "Scriptable Objects/ShopItemsRarityGroup")]
public class ShopItemsRarityGroup : ScriptableObject
{
    [field : SerializeField] public Item[] Items { get; private set; }
}
