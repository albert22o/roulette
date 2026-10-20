using UnityEngine;
using Zenject;

public class ShopChipPlacesController : MonoBehaviour
{
    [SerializeField] private ChipShopPlace[] shopPlaces;
    [SerializeField] private ShopItemsTable itemsTable;

    private ShopItemsPool itemsPool;

    private void Awake()
    {
        itemsPool = new ShopItemsPool(itemsTable);
    }

    public void Fill()
    {
        foreach (ChipShopPlace place in shopPlaces)
        {
            place.SetItem(itemsPool.GetRandomItem() as ChipData);
        }
    }

    public void Reroll()
    {
        foreach (ChipShopPlace place in shopPlaces)
        {
            if (place.Item != null)
            {
                place.SetItem(itemsPool.GetRandomItem() as ChipData);
            }
        }
    }

    public bool CheckRerollAvailable()
    {
        bool hasItem = false;
        foreach (ChipShopPlace place in shopPlaces)
        {
            if (place.IsAnimationPlaying) return false;
            if (place.Item != null) hasItem = true;
        }
        if (hasItem == false) return false;
        return true;
    }
}
