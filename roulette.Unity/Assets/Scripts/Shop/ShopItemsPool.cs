using UnityEngine;

public class ShopItemsPool
{
    private ShopItemsTable shopItemsTable;

    public void SetTable(ShopItemsTable table)
    {
        shopItemsTable = table;
    }

    public ItemData GetRandomItem()
    {
        var groups = shopItemsTable.Tables;

        float random = Random.value * 100f;

        ShopItemsRarityGroup selectedGroup = null;

        foreach (var g in groups)
        {
            if (random < g.Rarity)
            {
                selectedGroup = g.RarityGroup;
                break;
            }
            random -= g.Rarity;
        }


        var items = selectedGroup.Items;

        if (items == null || items.Length == 0)
        {
            Debug.LogWarning($"Rarity group {selectedGroup.name} has no items!");
            return null;
        }

        return items[Random.Range(0, items.Length)];
}
}
