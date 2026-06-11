using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallResultCardUI : MonoBehaviour
{
    public HitInfo HitInfo => hitInfo;

    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] TextMeshProUGUI colorText;
    [SerializeField] TextMeshProUGUI dozenText;
    [SerializeField] Image background;
    [SerializeField] GameObject selectedHighlight;

    private HitInfo hitInfo;

    public void Init(HitInfo hit, Action onClick)
    {
        hitInfo = hit;
        numberText.text = hit.FieldNumber.ToString();
        colorText.text = hit.FieldColor;
        dozenText.text = hit.Dozen.ToString();
        background.color = ConvertToColor(hit.FieldColor);
        GetComponent<Button>().onClick.AddListener(() => onClick());
    }

    public void SetSelected(bool v)
    {
        selectedHighlight.SetActive(v);
    }

    private Color ConvertToColor(string fieldColor)
    {
        return fieldColor switch
        {
            "Red" => Color.red,
            "Black" => Color.black,
            _ => Color.white,
        };
    }
}