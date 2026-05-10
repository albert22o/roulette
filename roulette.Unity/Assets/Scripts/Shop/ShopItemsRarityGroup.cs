using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsRarityGroup", menuName = "Scriptable Objects/ShopItemsRarityGroup")]
public class ShopItemsRarityGroup : ScriptableObject
{
    [field : SerializeField] public ItemData[] Items { get; private set; }
}
