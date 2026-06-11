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
        colorText.text = hit.FieldColor.ToString();
        dozenText.text = hit.Dozen.ToString();
        background.color = ConvertToColor(hit.FieldColor);
        GetComponent<Button>().onClick.AddListener(() => onClick());
    }

    public void SetSelected(bool v)
    {
        selectedHighlight.SetActive(v);
    }

    private Color ConvertToColor(ColorType fieldColor)
    {
        return fieldColor switch
        {
            ColorType.Red => Color.red,
            ColorType.Black => Color.black,
            _ => Color.white,
        };
    }
}