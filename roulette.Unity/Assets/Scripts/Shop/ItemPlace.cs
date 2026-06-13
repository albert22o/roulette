using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    private ItemData item;
    private GameObject spawnedItem;

    public ItemData Item => item;
    
    public void SetItem(ItemData item)
    {
        if (spawnedItem != null)
            Destroy(spawnedItem.gameObject);
        if (item == null)
        {
            this.item = null;
            spawnedItem = null;
        }
        else
        {
            spawnedItem = Instantiate(item.Prefab, transform);
            spawnedItem.transform.position = transform.position;
            this.item = item;
        }
    }
}
