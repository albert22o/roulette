using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI textLabel;

    public void Setup(Sprite icon, string text)
    {
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false); 
            }
        }

        if (textLabel != null)
            textLabel.text = text;
    }
}