using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class BetButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEngine.Color? Color
    {
        get => BG != null ? BG.color : null;
        set
        {
            if (value != null && BG != null)
                BG.color = (UnityEngine.Color)value;
        }
    }
    public string FieldNum
    {
        get => text != null ? text.text : null;
        set
        {
            if (text != null)
                text.text = value;
        }
    }
    public string BetSum
    {
        get => betText != null ? betText.text : null;
        set
        {
            if (betText != null)
                betText.text = value;
        }
    }

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI betText;

    private Image BG;
    private event Action OnRightClick;
    private event Action OnLightClick;

    public void Init(BetInfo betInfo, Action leftClickCallback, Action rightClickCallback)
    {
        BG = GetComponent<Image>();
        Color = GetColor(betInfo.FieldColor);

        if (text == null)
            Debug.LogError("Text is not assigned in BetButton.");
        FieldNum = betInfo.FieldNumber.ToString();

        if (betText == null)
            Debug.LogError("BetText is not assigned in BetButton.");

        OnLightClick += leftClickCallback;
        OnRightClick += rightClickCallback;
    }

    private UnityEngine.Color? GetColor(string fieldColor) => fieldColor switch
    {
        "Red" => UnityEngine.Color.red,
        "Black" => UnityEngine.Color.black,
        _ => null
    };

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLightClick?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick?.Invoke();
    }
}