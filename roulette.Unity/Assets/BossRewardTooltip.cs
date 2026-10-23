using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossRewardTooltip : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<RarityType, Sprite> raritySprites;

    [SerializeField] private TextMeshProUGUI title, mainText;
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private ScreenSpaceTooltip tooltip;
    [SerializeField] private Image bg;


    public void Show(string title, string mainText, RarityType rarityType, Vector3 position)
    {
        tooltip.Show(position);
        this.title.text = title;
        this.mainText.text = mainText;
        bg.sprite = raritySprites[rarityType];
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainRect);
    }

    public void Hide()
    {
        tooltip.Hide();
    }
}
