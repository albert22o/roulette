using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.SmartFormat.Utilities;
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
        chipHolder.ChipAnimated += OnChipAnimated;

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
            chipHolder.ChipAdded -= OnChipAdded;
            chipHolder.ChipRemoved -= OnChipRemoved;
            chipHolder.ChipsSwapped -= OnChipsSwapped;
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
        draggedObject = slot.ChipObject;

        draggedFromSlot.DetachChip();
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

        chipA.transform.SetParent(null);
        chipB.transform.SetParent(null);

        slotViews[indexA].PlaceChip(chipB);
        slotViews[indexB].PlaceChip(chipA);

        var chipDataA = chipHolder.Chips[indexA];
        var chipDataB = chipHolder.Chips[indexB];

        slotViews[indexA].SetChipName(chipDataA?.ChipData?.Name ?? "");
        slotViews[indexB].SetChipName(chipDataB?.ChipData?.Name ?? "");

    }

    private void OnChipAnimated(int index)
    {
        slotViews[index].PlayAnimation();
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

   
}