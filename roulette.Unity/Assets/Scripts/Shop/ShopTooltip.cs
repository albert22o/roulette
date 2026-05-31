using TMPro;
using UnityEngine;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, mainText;

    public void SetData(string title, string mainText)
    {
        this.title.text = title;
        this.mainText.text = mainText;
    }

}   
