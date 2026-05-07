using TMPro;
using UnityEngine;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName, decription;

    public void SetItem(Item item)
    {
        itemName.text = item.NameKey;
        decription.text = item.DescriptionKey;
    }
}   
