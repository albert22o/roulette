using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    private ItemData item;
    private GameObject spawnedItem;

    
    public void SetItem(ItemData item)
    {
        if (item == this.item) return;
        if (spawnedItem != null)
            Destroy(spawnedItem.gameObject);
        if (item == null)
        {
            this.item = null;
            spawnedItem = null;
        }
        else
        {
            spawnedItem = Instantiate(item.ShopPrefab, transform);
            spawnedItem.transform.position = transform.position;
            this.item = item;
        }
    }
}
