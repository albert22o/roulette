using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    private ItemData item;
    private ItemVisual spawnedItem;
    
    public void SetItem(ItemData item)
    {
        if (item == null)
        {
            if (spawnedItem != null)
                Destroy(spawnedItem.gameObject);
            this.item = null;
            spawnedItem = null;
        }
        else
        {
            spawnedItem = Instantiate(item.Prefab, transform);
            spawnedItem.transform.position = transform.position;
            spawnedItem.SetMaterial(item.VisualComponent.Material);
            this.item = item;
        }
    }
}
