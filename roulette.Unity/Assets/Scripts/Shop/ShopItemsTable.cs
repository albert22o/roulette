using NUnit.Framework;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsTable", menuName = "Scriptable Objects/ShopItemsTable")]
public class ShopItemsTable : ScriptableObject
{
    [SerializeField] private ItemsTableElement[] itemsTables;

    public ItemsTableElement[] Tables => itemsTables;

    private void OnValidate()
    {
        float sum = 0;
        for (int i = 0; i < itemsTables.Length; i++)
        {
            sum += itemsTables[i].Rarity;
        }
        if (sum != 100) Debug.LogWarning("Sum of chances in " + name + " don't equal 100");
    }

    public float GetProbability(RarityType rarityType)
    {
        if (itemsTables == null) return 0f;

        var match = itemsTables.FirstOrDefault(element =>
            element != null &&
            element.RarityGroup != null &&
            element.RarityGroup.RarityType == rarityType);

        if (match != null)
        {
            return match.Rarity;
        }

        Debug.LogWarning($"[ShopItemsTable] RarityType '{rarityType}' not found in '{name}'. Returning 0%.", this);
        return 0f;
    }
}

[System.Serializable]
public class ItemsTableElement
{
    public ShopItemsRarityGroup RarityGroup;
    [UnityEngine.Range(0, 100)]
    public float Rarity;
}
