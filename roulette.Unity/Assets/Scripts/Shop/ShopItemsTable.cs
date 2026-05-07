using NUnit.Framework;
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
}

[System.Serializable]
public class ItemsTableElement
{
    public ShopItemsRarityGroup RarityGroup;
    [UnityEngine.Range(0, 100)]
    public float Rarity;
}
