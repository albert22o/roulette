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

    [SerializeField] private ChipData[] chips;
    private ChipSlotView hoveredSlot;

    [Inject]
    public void Construct(ChipHolder holder)
    {
        chipHolder = holder;
    }

    public void Init(Camera mainCamera)
    {
        this.mainCamera = mainCamera;
        chipHolder.ChipAdded    += OnChipAdded;
        chipHolder.ChipRemoved  += OnChipRemoved;
        chipHolder.ChipsSwapped += OnChipsSwapped;

        InitSlots();
        SetupInput();

        foreach (var data in chips)
            chipHolder.AddChip(data);
    }

    private void OnDestroy()
    {
        if (chipHolder != null)
        {
            chipHolder.ChipAdded    -= OnChipAdded;
            chipHolder.ChipRemoved  -= OnChipRemoved;
            chipHolder.ChipsSwapped -= OnChipsSwapped;
        }

        clickAction?.Dispose();
        pointAction?.Dispose();
    }

    // ── Инициализация слотов ──────────────────────────────────────────

    private void InitSlots()
    {
        for (int i = 0; i < slotViews.Length; i++)
            slotViews[i].Init(i, slotViews[i].transform.position);
    }

    // ── Input ─────────────────────────────────────────────────────────

    private void SetupInput()
    {
        pointAction = new InputAction("Point", binding: "<Mouse>/position");
        pointAction.Enable();

        clickAction = new InputAction("Click", binding: "<Mouse>/leftButton");
        clickAction.started  += _ => TryBeginDrag();
        clickAction.canceled += _ => EndDrag();
        clickAction.Enable();
    }

    private void Update()
    {
        if (isDragging)
            UpdateDrag();
    }

    // ── Drag & Drop ───────────────────────────────────────────────────

    private void TryBeginDrag()
    {
        ChipSlotView slot = RaycastSlot();
        if (slot == null || slot.ChipObject == null) return;

        isDragging = true;
        draggedFromSlot = slot;
        draggedObject = slot.ChipObject;

        draggedFromSlot.DetachChip();
    }

    private void UpdateDrag()
    {
        Vector3 worldPos = GetMouseWorldPosition(dragHeight);
        if (worldPos == Vector3.zero) return;

        draggedObject.transform.position = worldPos;
        hoveredSlot = RaycastSlot(); // обновляем каждый кадр
    }

    private void EndDrag()
    {
        if (!isDragging) return;
        isDragging = false;

        ChipSlotView targetSlot = hoveredSlot; // берём последний известный
        hoveredSlot = null;

        if (targetSlot != null && targetSlot != draggedFromSlot)
        {
            draggedObject.transform.SetParent(draggedFromSlot.transform);
            draggedFromSlot.RestoreChip(draggedObject);
            chipHolder.SwapChips(draggedFromSlot.SlotIndex, targetSlot.SlotIndex);
        }
        else
        {
            draggedFromSlot.PlaceChip(draggedObject);
        }

        draggedFromSlot = null;
        draggedObject = null;
    }


    // ── Обработчики ивентов ChipHolder ────────────────────────────────

    private void OnChipAdded(int slotIndex, Chip chip)
    {
        GameObject chipObject = Instantiate(chip.ChipData.Prefab);
        slotViews[slotIndex].PlaceChip(chipObject);
    }

    private void OnChipRemoved(int slotIndex)
    {
        slotViews[slotIndex].ClearChip();
    }

    private void OnChipsSwapped(int indexA, int indexB)
    {
        GameObject chipA = slotViews[indexA].ChipObject;
        GameObject chipB = slotViews[indexB].ChipObject;

        chipA?.transform.SetParent(null);
        chipB?.transform.SetParent(null);

        slotViews[indexA].PlaceChip(chipB);
        slotViews[indexB].PlaceChip(chipA);
    }
    // ── Вспомогательное ───────────────────────────────────────────────

    private ChipSlotView RaycastSlot()
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayerMask)) return null;

        return hit.collider.GetComponentInParent<ChipSlotView>();
    }

    private Vector3 GetMouseWorldPosition(float height)
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        Plane tablePlane = new Plane(Vector3.up, new Vector3(0f, height, 0f));

        if (tablePlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }
}