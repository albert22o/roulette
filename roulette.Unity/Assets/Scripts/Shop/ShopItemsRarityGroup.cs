using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsRarityGroup", menuName = "Scriptable Objects/ShopItemsRarityGroup")]
public class ShopItemsRarityGroup : ScriptableObject
{
    [field : SerializeField] public ItemData[] Items { get; private set; }

    public ItemData GetRandomItem()
    {
        if (Items == null || Items.Length == 0)
        {
            Debug.LogWarning("ShopItemsRarityGroup has no items.");
            return null;
        }
        int randomIndex = Random.Range(0, Items.Length);
        return Items[randomIndex];
    }
}
