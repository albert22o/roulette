using TMPro;
using UnityEngine;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName, decription;

    public void SetItem(Item item)
    {
        itemName.text = item.Data.NameKey;
        decription.text = item.Data.DescriptionKey;
    }

    public void SetItemData(ItemData data)
    {
        itemName.text = data.NameKey;
        decription.text = data.DescriptionKey;
    }
}   
