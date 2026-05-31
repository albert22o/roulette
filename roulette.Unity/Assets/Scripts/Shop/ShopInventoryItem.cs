using UnityEngine;

public class ShopInventoryItem : MonoBehaviour
{
    [SerializeField] private ItemPlace place;
    public Item Item { get; private set;  }


    public void SetItem(Item item)
    {
        Item = item;
        place.SetItem(item.Data);
    }
}
