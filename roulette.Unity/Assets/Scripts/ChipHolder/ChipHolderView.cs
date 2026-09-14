using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ChipHolderView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChipSlotView[] slotViews;

    [Header("Layout")]
    [SerializeField] private float dragHeight = 0.5f;
    [SerializeField] private LayerMask slotLayerMask;

    private Camera mainCamera;
    private ChipHolder chipHolder;

    private ChipSlotView draggedFromSlot;
    private GameObject draggedObject;
    private bool isDragging;

    private InputAction clickAction;
    private InputAction pointAction;
    private ChipSlotView hoveredSlot;


    [Inject]
    public void Construct(ChipHolder holder, Camera mainCamera)
    {
        chipHolder = holder;
        this.mainCamera = mainCamera;
    }

    public void Start()
    {
        chipHolder.ChipAdded += OnChipAdded;
        chipHolder.ChipRemoved += OnChipRemoved;
        chipHolder.ChipsSwapped += OnChipsSwapped;
        chipHolder.ChipMoved += OnChipMoved;
        chipHolder.ChipAnimated += OnChipAnimated;
        chipHolder.SlotAdded += OnSlotAdded;
        chipHolder.SlotRemoved += OnSlotRemoved;

        InitSlots();
        SetupInput();

        for (int index = 0; index < chipHolder.Chips.Count; index++)
        {
            if (chipHolder.Chips[index] == null)
                continue;

            OnChipAdded(index, chipHolder.Chips[index]);
        }
    }

    private void OnDestroy()
    {
        if (chipHolder != null)
        {
            chipHolder.SlotAdded -= OnSlotAdded;
            chipHolder.SlotRemoved -= OnSlotRemoved;
            chipHolder.ChipAdded -= OnChipAdded;
            chipHolder.ChipRemoved -= OnChipRemoved;
            chipHolder.ChipsSwapped -= OnChipsSwapped;
            chipHolder.ChipMoved -= OnChipMoved;
        }

        clickAction?.Dispose();
        pointAction?.Dispose();
    }

    private void InitSlots()
    {
        for (int i = 0; i < slotViews.Length; i++)
            slotViews[i].Init(i, slotViews[i].transform.position);
    }

    private void SetupInput()
    {
        pointAction = new InputAction("Point", binding: "<Mouse>/position");
        pointAction.Enable();

        clickAction = new InputAction("Click", binding: "<Mouse>/leftButton");
        clickAction.started += _ => TryBeginDrag();
        clickAction.canceled += _ => EndDrag();
        clickAction.Enable();
    }

    private void Update()
    {
        if (isDragging)
            UpdateDrag();
    }

    private void TryBeginDrag()
    {
        var slot = RaycastSlot();
        if (slot == null || slot.ChipObject == null)
            return;

        isDragging = true;
        draggedFromSlot = slot;

        // DetachChip возвращает объект и сам обнуляет ChipObject у слота,
        // чтобы слот не считался занятым, пока фишка находится "в воздухе".
        draggedObject = draggedFromSlot.DetachChip();

        // Останавливаем Animator на время драга: Idle-анимация может писать
        // в transform (позицию/масштаб) поверх нашего ручного перемещения
        // и конфликтовать с ним, из-за чего фишка "пропадает"/дёргается.
        var animator = draggedObject.GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
    }

    private void UpdateDrag()
    {
        var worldPos = GetMouseWorldPosition(dragHeight);
        if (worldPos == Vector3.zero)
            return;

        draggedObject.transform.position = worldPos;
        hoveredSlot = RaycastSlot();
    }

    private void EndDrag()
    {
        if (!isDragging)
            return;
        isDragging = false;

        var targetSlot = hoveredSlot;
        hoveredSlot = null;

        // Включаем Animator обратно — драг закончен, фишка снова "лежит на месте"
        // и может проигрывать свою Idle-анимацию как обычно.
        var animator = draggedObject != null ? draggedObject.GetComponent<Animator>() : null;
        if (animator != null)
            animator.enabled = true;

        if (targetSlot != null && targetSlot != draggedFromSlot)
        {
            draggedObject.transform.SetParent(draggedFromSlot.transform);
            draggedFromSlot.RestoreChip(draggedObject);

            // chipHolder.SwapChips сам вызовет ChipsSwapped и там мы
            // обновим визуал обоих слотов (OnChipsSwapped безопасен
            // даже если целевой слот был пуст).
            chipHolder.SwapChips(draggedFromSlot.SlotIndex, targetSlot.SlotIndex);
        }
        else
        {
            draggedFromSlot.PlaceChip(draggedObject);
        }

        draggedFromSlot = null;
        draggedObject = null;
    }

    private void OnChipAdded(int slotIndex, Chip chip)
    {
        var chipObject = Instantiate(chip.ChipData.Prefab);
        slotViews[slotIndex].PlaceChip(chipObject);
        slotViews[slotIndex].SetChipName(chip.ChipData.Name);
    }

    private void OnChipRemoved(int slotIndex)
    {
        slotViews[slotIndex].ClearChip();
        slotViews[slotIndex].SetChipName("");
    }

    private void OnChipsSwapped(int indexA, int indexB)
    {
        var chipA = slotViews[indexA].ChipObject;
        var chipB = slotViews[indexB].ChipObject;

        // ВАЖНО: здесь нельзя использовать оператор "?." для UnityEngine.Object.
        // Unity переопределяет "==", чтобы корректно определять "fake null"
        // (объект уже уничтожен через Destroy(), но C#-ссылка формально не null).
        // Оператор "?." этот переопределённый "==" НЕ использует и делает
        // сырую проверку ссылки, поэтому уничтоженный объект ошибочно считается
        // "живым" — и обращение к chipA.transform кидает MissingReferenceException.
        // Поэтому используем явные "!= null" / "== null" сравнения.
        bool hasChipA = chipA != null;
        bool hasChipB = chipB != null;

        if (hasChipA)
            chipA.transform.SetParent(null);
        if (hasChipB)
            chipB.transform.SetParent(null);

        if (hasChipB)
            slotViews[indexA].PlaceChip(chipB);
        else
            slotViews[indexA].ClearReference(); // chipA переехал в indexB, не уничтожаем его

        if (hasChipA)
            slotViews[indexB].PlaceChip(chipA);
        else
            slotViews[indexB].ClearReference(); // chipB переехал в indexA, не уничтожаем его

        var chipDataA = chipHolder.Chips[indexA];
        var chipDataB = chipHolder.Chips[indexB];

        slotViews[indexA].SetChipName(chipDataA?.ChipData?.Name ?? "");
        slotViews[indexB].SetChipName(chipDataB?.ChipData?.Name ?? "");
    }

    /// <summary>
    /// Вызывается, когда ChipHolder сдвигает фишку в массиве данных
    /// (например, из-за RemoveSlot в середине holder'а — все фишки после
    /// удалённого слота сдвигаются на одну позицию назад). Переставляем
    /// визуальный объект фишки между соответствующими ChipSlotView, чтобы
    /// вид не рассинхронизировался с данными.
    /// </summary>
    private void OnChipMoved(int fromIndex, int toIndex)
    {
        var chipObject = slotViews[fromIndex].ChipObject;
        if (chipObject == null)
            return;

        chipObject.transform.SetParent(null);
        slotViews[fromIndex].ClearReference(); // фишка переезжает в toIndex, не уничтожаем её

        slotViews[toIndex].PlaceChip(chipObject);

        var chipData = chipHolder.Chips[toIndex];
        slotViews[toIndex].SetChipName(chipData?.ChipData?.Name ?? "");
    }

    private void OnChipAnimated(int index)
    {
        slotViews[index].PlayAnimation();
        slotViews[index].ShowMultiplier(chipHolder.Chips[index].AppliedValue, chipHolder.Chips[index].Operation);
    }

    private ChipSlotView RaycastSlot()
    {
        var screenPos = pointAction.ReadValue<Vector2>();
        var ray = mainCamera.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out var hit, 100f, slotLayerMask))
            return null;

        return hit.collider.GetComponentInParent<ChipSlotView>();
    }

    private Vector3 GetMouseWorldPosition(float height)
    {
        var screenPos = pointAction.ReadValue<Vector2>();
        var ray = mainCamera.ScreenPointToRay(screenPos);
        var tablePlane = new Plane(Vector3.up, new Vector3(0f, height, 0f));

        if (tablePlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }

    private void OnSlotAdded(int index)
    {
        if (index < slotViews.Length)
        {
            slotViews[index].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Нет ChipSlotView для индекса {index}. " +
                              "Нужно либо заранее подготовить достаточно слотов в сцене, " +
                              "либо реализовать динамический спавн ChipSlotView.");
        }
    }

    private void OnSlotRemoved(int index)
    {
        if (index < slotViews.Length)
        {
            slotViews[index].ClearChip();
            slotViews[index].SetChipName("");
            slotViews[index].gameObject.SetActive(false);
        }
    }
}