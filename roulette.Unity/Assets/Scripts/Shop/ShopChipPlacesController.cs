using UnityEngine;

public class ShopChipPlacesController : MonoBehaviour
{
    [SerializeField] private ShopPlace[] shopPlaces;
    [SerializeField] private ShopItemsTable itemsTable;

    private ShopItemsPool itemsPool = new();

    private void Start()
    {
        itemsPool.SetTable(itemsTable);
    }

    public void Fill()
    {
        foreach (ShopPlace place in shopPlaces)
        {
            place.SetItem(itemsPool.GetRandomItem());
        }
    }

    public void Reroll()
    {
        foreach (ShopPlace place in shopPlaces)
        {
            if (place.Item != null)
            {
                place.SetItem(itemsPool.GetRandomItem());
            }
        }
    }

    public bool CheckRerollAvailable()
    {
        bool hasItem = false;
        foreach (ShopPlace place in shopPlaces)
        {
            if (place.IsAnimationPlaying) return false;
            if (place.Item != null) hasItem = true;
        }
        if (hasItem == false) return false;
        return true;
    }
}
