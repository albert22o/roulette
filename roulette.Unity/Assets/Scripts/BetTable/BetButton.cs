using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using System.Collections;

public class BetButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int Index => index;
    public bool IsHighlighted { get; private set; }

    [SerializeField] TextMeshPro fieldNumText;
    [SerializeField] Transform spawnChipPoint;
    [SerializeField] GameObject chipPrefab;
    [SerializeField] Material blackMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material violetMaterial;

    [SerializeField] Material colorHighlight;
    [SerializeField] Material dozenHighlight;
    [SerializeField] Material numberHighlight;

    [SerializeField] private MeshRenderer BG;

    [SerializeField] private OutlineController outline;

    [Header("Chip spawn")]
    [SerializeField] private float spawnDelay = 0.05f; 
    [SerializeField] private float maxSpawnDuration = 1.5f; 

    private int targetChipCount;
    private Coroutine spawnRoutine;
    private readonly Stack<GameObject> stackChips = new();
    private ColorType color;
    private int index;
    private BetTable betTable;

    public event Action<BetButton> HoverEnter, HoverExit;

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
            targetChipCount = Mathf.Max(0, value / 25);

            while (stackChips.Count > targetChipCount)
                Destroy(stackChips.Pop());

            if (stackChips.Count < targetChipCount)
                StartSpawning();
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

    public event Action<BetButton> RightClick;
    public event Action<BetButton> LeftClick;

    private ChipSoundPlayer chipSoundPlayer;

    [Inject]
    public void Construct(BetTable table, ChipSoundPlayer chipSoundPlayer)
    {
        betTable = table;
        this.chipSoundPlayer = chipSoundPlayer;
    }

    private void SpawnChip()
    {
        GameObject chip = Instantiate(chipPrefab, spawnChipPoint);
        chip.transform.localRotation = Quaternion.identity;
        chipSoundPlayer.Attach(chip);
        stackChips.Push(chip);
    }
    public void Init(BetInfo betInfo, int index)
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

    }

    private void StartSpawning()
    {
        if (!isActiveAndEnabled)
        {
            while (stackChips.Count < targetChipCount)
                SpawnChip();
            return;
        }

        spawnRoutine ??= StartCoroutine(SpawnChipsRoutine());
    }

    private IEnumerator SpawnChipsRoutine()
    {
        while (stackChips.Count < targetChipCount)
        {
            SpawnChip();

            int remaining = targetChipCount - stackChips.Count;
            if (remaining <= 0)
                break;

            yield return new WaitForSeconds(Mathf.Min(spawnDelay, maxSpawnDuration / remaining));
        }

        spawnRoutine = null;
    }

    private void OnEnable()
    {
        if (stackChips.Count < targetChipCount)
            StartSpawning();
    }

    private void OnDisable()
    {
        spawnRoutine = null;
    }

    private Material GetColorMaterial(ColorType fieldColor) => fieldColor switch
    {
        ColorType.Red => redMaterial,
        ColorType.Black => blackMaterial,
        ColorType.Green => greenMaterial,
        ColorType.White => whiteMaterial,
        ColorType.Violet => violetMaterial,
        _ => greenMaterial,
    };

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftClick?.Invoke(this);
        else if (eventData.button == PointerEventData.InputButton.Right)
            RightClick?.Invoke(this);
        outline.ToggleMaterial(betTable.CanHighlight(index));
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
        if (betTable.CanHighlight(index))
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