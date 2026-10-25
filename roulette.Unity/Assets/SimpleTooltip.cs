using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenSpaceTooltip))]
public class SimpleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, mainText;
    [SerializeField] private RectTransform titleRect, mainTextRect, tooltipRect;
    [SerializeField] private ScreenSpaceTooltip worldSpaceTooltip;

    public void Show(string title, string  mainText, Vector3 position)
    {
        this.title.text = title;
        this.mainText.text = mainText;
        worldSpaceTooltip.Show(position);
        LayoutRebuilder.ForceRebuildLayoutImmediate(titleRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainTextRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
    }

    public void Hide()
    {
        worldSpaceTooltip.Hide();
    }
}
