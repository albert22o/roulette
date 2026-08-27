using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
}
