using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<RarityType, TooltipRarityDisplayData> rarityDisplayData;

    [SerializeField] private TextMeshProUGUI title, mainText, cost;
    [SerializeField] private RectTransform costRect, mainRect;
    [SerializeField] private ScreenSpaceTooltip screenSpaceTooltip;
    [SerializeField] private Image bg, costPlace;


    public void Show(string title, string mainText,string cost, RarityType rarityType, Vector3 position)
    {
        screenSpaceTooltip.Show(position);
        this.title.text = title;
        this.mainText.text = mainText;
        this.cost.text = cost;
        bg.sprite = rarityDisplayData[rarityType].Bg;
        costPlace.sprite = rarityDisplayData[rarityType].CostPlace;
        LayoutRebuilder.ForceRebuildLayoutImmediate(costRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainRect);
    }

    public void Hide()
    {
        screenSpaceTooltip.Hide();
    }
}

[System.Serializable]
public class TooltipRarityDisplayData
{
    [SerializeField] private Sprite bg, costPlace;

    public Sprite Bg => bg;
    public Sprite CostPlace => costPlace;
}
