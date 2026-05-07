using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopPlace[] shopPlaces;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private ShopItemsTable itemsTable;
    [SerializeField] private float tooltipHeight;

    private ShopItemsPool itemsPool = new ();
    private GoldenChipsController goldenChipsController;


    private void Start()
    {
        goldenChipsController = FindAnyObjectByType<GoldenChipsController>();
        itemsPool.SetTable(itemsTable);
        FillShopPlaces();
    }

    private void FillShopPlaces()
    {
        foreach( ShopPlace place in shopPlaces)
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
        if (goldenChipsController.Count >= place.Item.Cost)
        {
            goldenChipsController.RemoveChips(place.Item.Cost);
            Debug.Log(place.Item.NameKey + " bought!");
            return true;
        }
        else
        {
            Debug.Log(place.Item.NameKey + " not enough money!");
            return false;
        }
    }
}
