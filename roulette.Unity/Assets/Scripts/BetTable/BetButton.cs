using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(SpriteRenderer))]
public class BetButton : MonoBehaviour, IPointerClickHandler
{
    public string Color
    {
        get
        {
            return color;
        }
        set
        {
            if (value != null)
            {
                color = value;
                BG.color = GetColor(value);
            }
        }
    }
    public int FieldNum
    {
        get
        {
            return Convert.ToInt32(fieldNumText.text);
        }
        set
        {
            fieldNumText.text = value.ToString();
        }
    }
    public int Amount
    {
        set
        {
            var targetChipCount = Mathf.Max(0, value / 25);
            var currentCount = stackChips.Count;
            if (targetChipCount > currentCount)
            {
                while (targetChipCount != currentCount)
                {
                    stackChips.Push(Instantiate(chipPrefab, spawnChipPoint));
                    currentCount++;
                }
            }
            else if (targetChipCount < currentCount)
            {
                while (targetChipCount != currentCount)
                {
                    Destroy(stackChips.Pop());
                    currentCount--;
                }
            }
        }
    }

    public bool IsHighlighted { get; private set; }

    [SerializeField] TextMeshPro fieldNumText;
    [SerializeField] Transform spawnChipPoint;
    [SerializeField] GameObject chipPrefab;
    [SerializeField] Color colorHighlight;
    [SerializeField] Color dozenHighlight;
    [SerializeField] Color numberHighlight;
    [SerializeField] Color lowlight;

    private SpriteRenderer BG;
    private event Action OnRightClick;
    private event Action OnLeftClick;
    private readonly Stack<GameObject> stackChips = new();
    private string color;

    [SerializeField] private Color customGreen = new Color(0.11f, 0.37f, 0.13f, 1f);
    public void Init(BetInfo betInfo, Action leftClickCallback, Action rightClickCallback)
    {
        BG = GetComponent<SpriteRenderer>();
        Color = betInfo.FieldColor;

        if (fieldNumText == null)
            Debug.LogError("Text is not assigned in BetButton.");
        FieldNum = betInfo.FieldNumber;

        if (spawnChipPoint == null)
            Debug.LogError("BetText is not assigned in BetButton.");
        if (chipPrefab == null)
            Debug.LogError("ChipPrefab is not assigned in BetButton");

        OnLeftClick += leftClickCallback;
        OnRightClick += rightClickCallback;
    }

    private Color GetColor(string fieldColor) => fieldColor switch
    {
        "Red" => UnityEngine.Color.red,
        "Black" => UnityEngine.Color.black,
        "Green" => customGreen,
        _ => UnityEngine.Color.white,
    };

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick?.Invoke();
    }

    public void Highlight(bool isLowlight, bool isOn, int phase)
    {
        if (isOn)
        {
            if (isLowlight && !IsHighlighted)
            {
                BG.color = lowlight;
                return;
            }

            switch (phase)
            {
                case 1:
                    BG.color = colorHighlight;
                    break;
                case 2:
                    BG.color = dozenHighlight;
                    break;
                case 3:
                    BG.color = numberHighlight;
                    break;
                default:
                    BG.color = lowlight;
                    break;
            }
            IsHighlighted = true;
        }   
        else
        {
            IsHighlighted = false;
            BG.color = GetColor(color);
        }

    }

    public DozenType GetDozen()
    {
        DozenType dozenType;

        switch ((FieldNum + 11) / 12)
        {
            case 1:
                dozenType = DozenType.First;
                break;
            case 2:
                dozenType = DozenType.Second;
                break;
            case 3:
                dozenType = DozenType.Third;
                break;
            default:
                dozenType = DozenType.Zero;
                break;
        }

        return dozenType;
    }
}