using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(SpriteRenderer))]
public class BetButton : MonoBehaviour, IPointerClickHandler
{
    public ColorType Color
    {
        get => color;
        set
        {
           
            color = value;
            BG.color = GetColor(value);
            
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

    public DozenType Dozen 
    {
        get
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
    private ColorType color;

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

    private Color GetColor(ColorType fieldColor) => fieldColor switch
    {
        ColorType.Red => UnityEngine.Color.red,
        ColorType.Black => UnityEngine.Color.black,
        ColorType.Green => customGreen,
        _ => UnityEngine.Color.white,
    };

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick?.Invoke();
    }

    public void Highlight(bool isEnabled, HighlightPhase phase)
    {
        if (isEnabled)
        {
            switch (phase)
            {
                case HighlightPhase.Color:
                    BG.color = colorHighlight;
                    break;
                case HighlightPhase.Dozen:
                    BG.color = dozenHighlight;
                    break;
                case HighlightPhase.Number:
                    BG.color = numberHighlight;
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

    public void Lowlight(bool isEnabled)
    {
        if (isEnabled && !IsHighlighted)
            BG.color = lowlight;
    }

    public enum HighlightPhase 
    { 
        Color,
        Dozen,
        Number,
        None
    }
}