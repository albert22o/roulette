using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, mainText;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Show(string title, string mainText)
    {
        gameObject.SetActive(true);
        this.title.text = title;
        this.mainText.text = mainText;
        this.title.ForceMeshUpdate();
        this.mainText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.title.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.mainText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}   
