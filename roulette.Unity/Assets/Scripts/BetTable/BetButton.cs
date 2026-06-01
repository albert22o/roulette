using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(SpriteRenderer))]
public class BetButton : MonoBehaviour, IPointerClickHandler
{

    public string Color
    {
        set
        {
            if (value != null)
                BG.color = GetColor(value);
        }
    }
    public int FieldNum
    {
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

    [SerializeField] TextMeshPro fieldNumText;
    [SerializeField] Transform spawnChipPoint;
    [SerializeField] GameObject chipPrefab;

    private SpriteRenderer BG;
    private event Action OnRightClick;
    private event Action OnLeftClick;
    private readonly Stack<GameObject> stackChips = new();

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
        _ => customGreen,
    };

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick?.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick?.Invoke();
    }
}