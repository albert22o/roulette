using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopPlace[] shopPlaces;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private ShopItemsTable itemsTable;
    [SerializeField] private float tooltipHeight;

    private ShopItemsPool itemsPool = new();
    private Inventory inventory;


    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void Start()
    {
        itemsPool.SetTable(itemsTable);
        FillShopPlaces();
    }

    private void FillShopPlaces()
    {
        foreach (ShopPlace place in shopPlaces)
        {
            place.SetItem(itemsPool.GetRandomItem());
        }
    }

    public void Reroll()
    {

    }

    public void ShowDescription(ShopPlace place)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetItem(place.Item);
        tooltip.transform.position = place.transform.position + Vector3.up * tooltipHeight;
    }

    public void HideDescription()
    {
        tooltip.gameObject.SetActive(false);
    }

    public bool TryBuy(ShopPlace place)
    {
        if (inventory.GetCurrency(CurrencyType.GoldenChips) >= place.Item.Cost)
        {
            if (inventory.TryAddItem(new Item(place.Item)))
            {
                inventory.RemoveCurrency(CurrencyType.GoldenChips, place.Item.Cost);
                Debug.Log(place.Item.NameKey + " bought!");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log(place.Item.NameKey + " not enough money!");
            return false;
        }
    }
}
