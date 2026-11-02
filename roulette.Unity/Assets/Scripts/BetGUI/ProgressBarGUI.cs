using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ProgressBarGUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private bool showText = true;
    public void SetValue(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
        if (showText) tmp.text = $"{currentValue}/{maxValue}";
    }

    public void SetValueInPercent(float value)
    {
        slider.value = value * 0.01f;
        if (showText) tmp.text = $"{value}%";
    }

    public void SetCurrentAndMaxValue(long currentValue, long maxValue)
    {
        slider.value = (float) currentValue / maxValue;
        if (showText) tmp.text = $"{currentValue}/{maxValue}";
    }
}
