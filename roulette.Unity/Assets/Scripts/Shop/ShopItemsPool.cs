using UnityEngine;

public class ShopItemsPool
{
    private ShopItemsTable shopItemsTable;

    public ShopItemsPool(ShopItemsTable table)
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
        return selectedGroup.GetRandomItem();
    }
}
