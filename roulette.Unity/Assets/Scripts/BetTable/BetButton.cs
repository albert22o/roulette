using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


public class BetButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ColorType Color
    {
        get => color;
        set
        {
            color = value;
            BG.material = GetColorMaterial(value);
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
                    GameObject chip = Instantiate(chipPrefab, spawnChipPoint);
                    chip.transform.localRotation = Quaternion.Euler(0,0,0);
                    stackChips.Push(chip);

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
            var dozenType = ((FieldNum + 11) / 12) switch
            {
                1 => DozenType.First,
                2 => DozenType.Second,
                3 => DozenType.Third,
                _ => DozenType.Zero,
            };
            return dozenType;
        }
    }

    public bool IsHighlighted { get; private set; }

    [SerializeField] TextMeshPro fieldNumText;
    [SerializeField] Transform spawnChipPoint;
    [SerializeField] GameObject chipPrefab;
    [SerializeField] Material blackMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material greyMaterial;

    [SerializeField] Material colorHighlight;
    [SerializeField] Material dozenHighlight;
    [SerializeField] Material numberHighlight;

    [SerializeField] private MeshRenderer BG;

    [SerializeField] private OutlineController outline;
    private event Action OnRightClick;
    private event Action OnLeftClick;
    private readonly Stack<GameObject> stackChips = new();
    private ColorType color;
    private int index;
    private BetTable betTable;

    public event Action<BetButton> HoverEnter, HoverExit;

    [Inject]
    private void Construct(BetTable table)
    {
        this.betTable = table;
    }

    [SerializeField] private Color customGreen = new Color(0.11f, 0.37f, 0.13f, 1f);
    public void Init(BetInfo betInfo, Action leftClickCallback, Action rightClickCallback, int index)
    {
        this.index = index; 
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

    private Material GetColorMaterial(ColorType fieldColor) => fieldColor switch
    {
        ColorType.Red => redMaterial,
        ColorType.Black => blackMaterial,
        ColorType.Green => greenMaterial,
        ColorType.White => whiteMaterial,
        ColorType.Grey => greyMaterial,
        _ => greenMaterial,
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
                    BG.material = colorHighlight;
                    break;
                case HighlightPhase.Dozen:
                    BG.material = dozenHighlight;
                    break;
                case HighlightPhase.Number:
                    BG.material = numberHighlight;
                    break;
            }
            IsHighlighted = true;
        }   
        else
        {
            IsHighlighted = false;
            BG.material = GetColorMaterial(color);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverExit?.Invoke(this);
        outline.ToggleMaterial(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEnter?.Invoke(this);
        if (betTable.CanBet(index))
            outline.ToggleMaterial(true);
    }

    public enum HighlightPhase 
    { 
        Color,
        Dozen,
        Number,
        None
    }
}