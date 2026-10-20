using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

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
        get => Convert.ToInt32(fieldNumText.text);
        set => fieldNumText.text = value.ToString();
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

    [Inject]
    public void Construct(BetTable table)
    {
        this.betTable = table;
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

    // ---- Новые методы для работы с фишками ----

    /// <summary>Возвращает список текущих (активных) фишек на кнопке.</summary>
    public List<GameObject> GetActiveChips()
    {
        var result = new List<GameObject>(stackChips.Count);
        foreach (var chip in stackChips)
        {
            if (chip != null)
                result.Add(chip);
        }
        return result;
    }

    /// <summary>Мгновенно удаляет все фишки и сбрасывает счётчик.</summary>
    public void ClearChipsImmediately()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        while (stackChips.Count > 0)
        {
            var chip = stackChips.Pop();
            if (chip != null)
                Destroy(chip);
        }

        targetChipCount = 0;
    }

    /// <summary>
    /// Запускает горение на всех фишках. Возвращает максимальную длительность горения
    /// (или 0, если гореть нечему).
    /// </summary>
    public float BurnAllChips(Material burnMaterial, float burnDuration, string dissolveProperty)
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        var chips = GetActiveChips();
        if (chips.Count == 0)
        {
            targetChipCount = 0;
            return 0f;
        }

        // Новые фишки во время горения не нужны
        targetChipCount = 0;

        float maxDuration = 0f;
        foreach (var chip in chips)
        {
            if (chip == null) continue;

            var burnEffect = chip.GetComponentInChildren<BurnEffect>();
            Action onComplete = null;

            if (burnEffect == null)
            {
                var temp = chip.AddComponent<BurnEffect>();
                temp.Configure(burnMaterial, burnDuration, dissolveProperty);
                burnEffect = temp;
                onComplete = () => Destroy(temp);
            }

            burnEffect.StartBurn(onComplete);

            if (burnDuration > maxDuration)
                maxDuration = burnDuration;
        }

        // После горения фишки нужно убрать из стека и уничтожить
        StartCoroutine(FinalizeBurnedChips(maxDuration));
        return maxDuration;
    }

    private IEnumerator FinalizeBurnedChips(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (stackChips.Count > 0)
        {
            var chip = stackChips.Pop();
            if (chip != null)
                Destroy(chip);
        }
    }

    // ---- Конец новых методов ----

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

    private void SpawnChip()
    {
        GameObject chip = Instantiate(chipPrefab, spawnChipPoint);
        chip.transform.localRotation = Quaternion.identity;
        stackChips.Push(chip);
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
    public void BurnChips(Material burnMaterial, float startDelay, float burnTime)
    {
        if (stackChips.Count == 0) return;

        var chipsToBurn = new List<GameObject>();
        while (stackChips.Count > 0)
        {
            chipsToBurn.Add(stackChips.Pop());
        }

        StartCoroutine(BurnRoutine(chipsToBurn, burnMaterial, startDelay, burnTime));
    }

    private IEnumerator BurnRoutine(List<GameObject> chips, Material burnMaterial, float startDelay, float burnTime)
    {
        yield return new WaitForSeconds(startDelay);

        foreach (var chip in chips)
        {
            if (chip == null) continue;
            var renderer = chip.GetComponentInChildren<Renderer>();
            if (renderer != null && burnMaterial != null)
            {
                renderer.material = burnMaterial;
            }
        }

        yield return new WaitForSeconds(burnTime);

        foreach (var chip in chips)
        {
            if (chip != null) Destroy(chip);
        }
    }

    public enum HighlightPhase
    {
        Color,
        Dozen,
        Number,
        None
    }
}