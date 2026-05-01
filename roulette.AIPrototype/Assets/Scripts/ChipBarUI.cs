using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DeadlyRoulette
{
    /// <summary>
    /// Displays equipped chips in a horizontal bar. Supports drag-and-drop reordering
    /// during the Betting phase. Shows tooltip on hover.
    /// </summary>
    public class ChipBarUI : MonoBehaviour
    {
        [SerializeField] RouletteGame game;

        [Header("Chip Slots (pre-created, max ~10)")]
        [SerializeField] GameObject[] chipSlots;

        [Header("Tooltip")]
        [SerializeField] GameObject tooltip;
        [SerializeField] Text tooltipName;
        [SerializeField] Text tooltipDesc;

        void OnEnable()
        {
            if (game != null) game.OnStateChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (game != null) game.OnStateChanged -= Refresh;
        }

        void Start() { Refresh(); }

        void Refresh()
        {
            if (game == null) return;
            var inv = game.inventory;

            for (int i = 0; i < chipSlots.Length; i++)
            {
                if (chipSlots[i] == null) continue;
                bool active = i < inv.equippedChips.Count && !inv.equippedChips[i].Destroyed;
                chipSlots[i].SetActive(active);

                if (active)
                {
                    var chip = inv.equippedChips[i];
                    var def = ItemRegistry.Chips[chip.Id];

                    // Label
                    var label = chipSlots[i].GetComponentInChildren<Text>();
                    if (label != null) label.text = def.Name;

                    // Ensure handlers
                    var handler = chipSlots[i].GetComponent<ChipSlotHandler>();
                    if (handler == null) handler = chipSlots[i].AddComponent<ChipSlotHandler>();
                    handler.Setup(this, i, chip);
                }
            }

            if (tooltip) tooltip.SetActive(false);
        }

        public void ShowTooltip(ChipInstance chip)
        {
            if (tooltip == null) return;
            var def = ItemRegistry.Chips[chip.Id];
            if (tooltipName) tooltipName.text = def.Name;
            if (tooltipDesc) tooltipDesc.text = def.Description;
            tooltip.SetActive(true);
        }

        public void HideTooltip()
        {
            if (tooltip) tooltip.SetActive(false);
        }

        public void OnReorder(int from, int to)
        {
            if (game != null) game.ReorderChip(from, to);
        }

        public bool CanReorder => game != null && game.Phase == GamePhase.Betting;
    }

    /// <summary>
    /// Handles hover tooltip and drag-and-drop for a single chip slot.
    /// </summary>
    public class ChipSlotHandler : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        ChipBarUI bar;
        int slotIndex;
        ChipInstance chip;

        RectTransform rect;
        Canvas canvas;
        Vector3 originalPos;
        Transform originalParent;
        int originalSiblingIndex;

        public void Setup(ChipBarUI barUI, int index, ChipInstance chipInst)
        {
            bar = barUI;
            slotIndex = index;
            chip = chipInst;
            rect = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData e)
        {
            if (bar != null && chip != null) bar.ShowTooltip(chip);
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (bar != null) bar.HideTooltip();
        }

        public void OnBeginDrag(PointerEventData e)
        {
            if (bar == null || !bar.CanReorder) return;
            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            originalPos = rect.position;
            originalParent = rect.parent;
            originalSiblingIndex = rect.GetSiblingIndex();

            // Bring to front
            rect.SetParent(canvas.transform, true);
            rect.SetAsLastSibling();

            var group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (bar == null || !bar.CanReorder) return;
            rect.position += (Vector3)e.delta;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (bar == null) return;

            // Find drop target
            int targetIndex = slotIndex;
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(e, results);
            foreach (var r in results)
            {
                var targetHandler = r.gameObject.GetComponent<ChipSlotHandler>();
                if (targetHandler != null && targetHandler != this)
                {
                    targetIndex = targetHandler.slotIndex;
                    break;
                }
            }

            // Restore position
            rect.SetParent(originalParent, false);
            rect.SetSiblingIndex(originalSiblingIndex);
            rect.position = originalPos;

            var group = GetComponent<CanvasGroup>();
            if (group) group.blocksRaycasts = true;

            if (targetIndex != slotIndex)
                bar.OnReorder(slotIndex, targetIndex);
        }
    }
}
