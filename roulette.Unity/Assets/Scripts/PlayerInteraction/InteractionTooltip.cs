using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class InteractionTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void ShowTooltip(string message)
    {
        text.text = "[E] - " + message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
