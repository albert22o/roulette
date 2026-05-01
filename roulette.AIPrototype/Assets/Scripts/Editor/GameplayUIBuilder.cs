using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor utility: creates the Chip Bar and Ball Display UI hierarchies.
/// Run via menu Tools > Build Gameplay UI. Safe to delete after use.
/// </summary>
public class GameplayUIBuilder
{
    static Font font;
    static readonly Color BG_PANEL   = new Color(0.12f, 0.12f, 0.15f, 0.85f);
    static readonly Color BG_SLOT    = new Color(0.25f, 0.25f, 0.30f, 1f);
    static readonly Color BG_TOOLTIP = new Color(0.08f, 0.08f, 0.10f, 0.95f);
    static readonly Color BG_RESULT  = new Color(0.20f, 0.20f, 0.25f, 1f);
    static readonly Color BTN_SELECT = new Color(0.2f, 0.5f, 0.7f, 1f);

    [MenuItem("Tools/Build Gameplay UI")]
    public static void Build()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Find or create a canvas on the main game canvas
        var existingCanvas = Object.FindObjectOfType<Canvas>();
        Transform root;
        if (existingCanvas != null)
        {
            root = existingCanvas.transform;
        }
        else
        {
            var canvasGo = new GameObject("GameCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGo, "Build Gameplay UI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();
            root = canvasGo.transform;
        }

        BuildChipBar(root);
        BuildBallDisplay(root);

        Debug.Log("[GameplayUIBuilder] Chip Bar and Ball Display UI created. Assign references in ChipBarUI and BallDisplayUI components.");
    }

    // ==========================================================
    //  Chip Bar (bottom-center, horizontal)
    // ==========================================================

    static void BuildChipBar(Transform root)
    {
        // Container — full-width bar above the ResultBar
        var chipBarGo = CreatePanel(root, "ChipBar", BG_PANEL);
        var chipBarRT = chipBarGo.GetComponent<RectTransform>();
        chipBarRT.anchorMin = new Vector2(0f, 0f);
        chipBarRT.anchorMax = new Vector2(1f, 0f);
        chipBarRT.pivot = new Vector2(0.5f, 0f);
        chipBarRT.anchoredPosition = new Vector2(0, 80);
        chipBarRT.sizeDelta = new Vector2(0, 80);

        var layout = chipBarGo.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 6;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding = new RectOffset(8, 8, 6, 6);

        // Add ChipBarUI component
        var chipBarUI = chipBarGo.AddComponent<DeadlyRoulette.ChipBarUI>();

        // Create 10 chip slots
        var slots = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            var slot = CreatePanel(chipBarGo.transform, $"ChipSlot_{i}", BG_SLOT);
            // Layout groups need non-stretched anchors on children
            var slotRT = slot.GetComponent<RectTransform>();
            slotRT.anchorMin = new Vector2(0f, 0f);
            slotRT.anchorMax = new Vector2(0f, 1f);
            slotRT.pivot = new Vector2(0f, 0.5f);
            slotRT.anchoredPosition = Vector2.zero;
            slotRT.sizeDelta = new Vector2(90, 0);
            var slotLE = slot.AddComponent<LayoutElement>();
            slotLE.minWidth = 90;
            slotLE.preferredWidth = 90;
            slotLE.preferredHeight = 60;
            slotLE.flexibleWidth = 0;

            // Label
            var label = CreateText(slot.transform, "Label", "Chip", 13, TextAnchor.MiddleCenter);
            var labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(4, 4);
            labelRT.offsetMax = new Vector2(-4, -4);

            // Add Image for drag highlight
            slot.GetComponent<Image>().raycastTarget = true;

            slots[i] = slot;
            slot.SetActive(false); // hidden by default
        }

        // Chip tooltip — floats just above the ChipBar (which sits at y=80..160)
        var chipTooltip = CreatePanel(root, "ChipTooltip", BG_TOOLTIP);
        var tooltipRT = chipTooltip.GetComponent<RectTransform>();
        tooltipRT.anchorMin = new Vector2(0.3f, 0f);
        tooltipRT.anchorMax = new Vector2(0.7f, 0f);
        tooltipRT.pivot = new Vector2(0.5f, 0f);
        tooltipRT.anchoredPosition = new Vector2(0, 170);
        tooltipRT.sizeDelta = new Vector2(0, 60);

        var tooltipLayout = chipTooltip.AddComponent<VerticalLayoutGroup>();
        tooltipLayout.padding = new RectOffset(10, 10, 6, 6);
        tooltipLayout.childControlWidth = true;
        tooltipLayout.childControlHeight = true;
        tooltipLayout.childForceExpandWidth = true;
        tooltipLayout.childForceExpandHeight = false;

        var chipTTName = CreateText(chipTooltip.transform, "TooltipName", "Chip Name", 15, TextAnchor.MiddleCenter);
        chipTTName.fontStyle = FontStyle.Bold;
        NormalizeVerticalChild(chipTTName.gameObject, 22);
        var chipTTDesc = CreateText(chipTooltip.transform, "TooltipDesc", "Description", 12, TextAnchor.MiddleCenter);
        NormalizeVerticalChild(chipTTDesc.gameObject, 20);

        chipTooltip.SetActive(false);

        // Assign serialized fields via SerializedObject
        var so = new SerializedObject(chipBarUI);
        var slotsArray = so.FindProperty("chipSlots");
        slotsArray.arraySize = slots.Length;
        for (int i = 0; i < slots.Length; i++)
            slotsArray.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
        so.FindProperty("tooltip").objectReferenceValue = chipTooltip;
        so.FindProperty("tooltipName").objectReferenceValue = chipTTName;
        so.FindProperty("tooltipDesc").objectReferenceValue = chipTTDesc;
        so.ApplyModifiedProperties();
    }

    // ==========================================================
    //  Ball Display (bottom-left for owned, center for results)
    // ==========================================================

    static void BuildBallDisplay(Transform root)
    {
        // BallDisplayUI component on a container
        var ballDisplayGo = new GameObject("BallDisplay");
        ballDisplayGo.transform.SetParent(root, false);
        var ballDisplayUI = ballDisplayGo.AddComponent<DeadlyRoulette.BallDisplayUI>();

        // --- Owned Balls Panel (right column) ---
        var ownedPanel = CreatePanel(root, "OwnedBallsPanel", BG_PANEL);
        var ownedRT = ownedPanel.GetComponent<RectTransform>();
        ownedRT.anchorMin = new Vector2(0.87f, 0.23f);
        ownedRT.anchorMax = new Vector2(1f, 0.90f);
        ownedRT.pivot = new Vector2(0.5f, 0.5f);
        ownedRT.anchoredPosition = Vector2.zero;
        ownedRT.sizeDelta = Vector2.zero;

        var ownedLayout = ownedPanel.AddComponent<VerticalLayoutGroup>();
        ownedLayout.spacing = 4;
        ownedLayout.childControlWidth = true;
        ownedLayout.childControlHeight = true;
        ownedLayout.childForceExpandWidth = true;
        ownedLayout.childForceExpandHeight = false;
        ownedLayout.padding = new RectOffset(6, 6, 6, 6);

        // Title
        var ownedTitle = CreateText(ownedPanel.transform, "Title", "Balls", 14, TextAnchor.MiddleCenter);
        ownedTitle.fontStyle = FontStyle.Bold;
        NormalizeVerticalChild(ownedTitle.gameObject, 24);

        // 6 ball slots
        var ballSlots = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            var slot = CreatePanel(ownedPanel.transform, $"BallSlot_{i}", BG_SLOT);
            NormalizeVerticalChild(slot, 60);

            var label = CreateText(slot.transform, "Label", "Ball", 12, TextAnchor.MiddleCenter);
            var lrt = label.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = new Vector2(4, 2);
            lrt.offsetMax = new Vector2(-4, -2);

            slot.GetComponent<Image>().raycastTarget = true;
            ballSlots[i] = slot;
            slot.SetActive(false);
        }

        // --- Spin Results Panel (left column, top half) ---
        var resultsPanel = CreatePanel(root, "SpinResultsPanel", BG_PANEL);
        var resultsRT = resultsPanel.GetComponent<RectTransform>();
        resultsRT.anchorMin = new Vector2(0f, 0.575f);
        resultsRT.anchorMax = new Vector2(0.2f, 0.90f);
        resultsRT.pivot = new Vector2(0.5f, 0.5f);
        resultsRT.anchoredPosition = Vector2.zero;
        resultsRT.sizeDelta = Vector2.zero;

        var resultsLayout = resultsPanel.AddComponent<VerticalLayoutGroup>();
        resultsLayout.spacing = 6;
        resultsLayout.childControlWidth = true;
        resultsLayout.childControlHeight = true;
        resultsLayout.childForceExpandWidth = true;
        resultsLayout.childForceExpandHeight = false;
        resultsLayout.padding = new RectOffset(8, 8, 8, 8);

        // Title
        var resultsTitle = CreateText(resultsPanel.transform, "Title", "Spin Results", 15, TextAnchor.MiddleCenter);
        resultsTitle.fontStyle = FontStyle.Bold;
        NormalizeVerticalChild(resultsTitle.gameObject, 26);

        // 6 result slots
        var resultSlots = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            var slot = CreatePanel(resultsPanel.transform, $"ResultSlot_{i}", BG_RESULT);
            NormalizeVerticalChild(slot, 50);
            slot.GetComponent<Image>().raycastTarget = true;

            // Horizontal content
            var hlayout = slot.AddComponent<HorizontalLayoutGroup>();
            hlayout.spacing = 6;
            hlayout.childControlWidth = true;
            hlayout.childControlHeight = true;
            hlayout.childForceExpandWidth = true;
            hlayout.childForceExpandHeight = true;
            hlayout.padding = new RectOffset(6, 6, 4, 4);

            // Ball name text
            var nameText = CreateText(slot.transform, "BallName", "Ball", 13, TextAnchor.MiddleLeft);
            NormalizeHorizontalChild(nameText.gameObject, 0, flexibleWidth: 1);

            // Number text
            var numText = CreateText(slot.transform, "NumberText", "0", 14, TextAnchor.MiddleCenter);
            numText.fontStyle = FontStyle.Bold;
            NormalizeHorizontalChild(numText.gameObject, 40);

            // Select button
            var btnGo = new GameObject("SelectBtn");
            btnGo.transform.SetParent(slot.transform, false);
            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = BTN_SELECT;
            var btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = btnImg;
            NormalizeHorizontalChild(btnGo, 60);

            var btnLabel = CreateText(btnGo.transform, "Label", "Select", 11, TextAnchor.MiddleCenter);
            var btnLabelRT = btnLabel.GetComponent<RectTransform>();
            btnLabelRT.anchorMin = Vector2.zero;
            btnLabelRT.anchorMax = Vector2.one;
            btnLabelRT.offsetMin = Vector2.zero;
            btnLabelRT.offsetMax = Vector2.zero;

            resultSlots[i] = slot;
            slot.SetActive(false);
        }

        // --- Ball Tooltip — floats just to the left of OwnedBallsPanel ---
        var ballTooltip = CreatePanel(root, "BallTooltip", BG_TOOLTIP);
        var btRT = ballTooltip.GetComponent<RectTransform>();
        btRT.anchorMin = new Vector2(0.87f, 0.55f);
        btRT.anchorMax = new Vector2(0.87f, 0.55f);
        btRT.pivot = new Vector2(1f, 0.5f);
        btRT.anchoredPosition = new Vector2(-5, 0);
        btRT.sizeDelta = new Vector2(220, 80);

        var btLayout = ballTooltip.AddComponent<VerticalLayoutGroup>();
        btLayout.padding = new RectOffset(8, 8, 6, 6);
        btLayout.childControlWidth = true;
        btLayout.childControlHeight = true;
        btLayout.childForceExpandWidth = true;
        btLayout.childForceExpandHeight = false;

        var ballTTName = CreateText(ballTooltip.transform, "TooltipName", "Ball Name", 14, TextAnchor.MiddleCenter);
        ballTTName.fontStyle = FontStyle.Bold;
        NormalizeVerticalChild(ballTTName.gameObject, 22);
        var ballTTDesc = CreateText(ballTooltip.transform, "TooltipDesc", "Description", 11, TextAnchor.MiddleCenter);
        NormalizeVerticalChild(ballTTDesc.gameObject, 20);

        ballTooltip.SetActive(false);
        resultsPanel.SetActive(false);

        // Assign serialized fields
        var so = new SerializedObject(ballDisplayUI);
        var ballSlotsArr = so.FindProperty("ballSlots");
        ballSlotsArr.arraySize = ballSlots.Length;
        for (int i = 0; i < ballSlots.Length; i++)
            ballSlotsArr.GetArrayElementAtIndex(i).objectReferenceValue = ballSlots[i];

        so.FindProperty("spinResultsPanel").objectReferenceValue = resultsPanel;

        var resultSlotsArr = so.FindProperty("resultSlots");
        resultSlotsArr.arraySize = resultSlots.Length;
        for (int i = 0; i < resultSlots.Length; i++)
            resultSlotsArr.GetArrayElementAtIndex(i).objectReferenceValue = resultSlots[i];

        so.FindProperty("tooltip").objectReferenceValue = ballTooltip;
        so.FindProperty("tooltipName").objectReferenceValue = ballTTName;
        so.FindProperty("tooltipDesc").objectReferenceValue = ballTTDesc;
        so.ApplyModifiedProperties();
    }

    // ==========================================================
    //  Helpers
    // ==========================================================

    static GameObject CreatePanel(Transform parent, string name, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    // Layout groups expect non-stretched children. CreatePanel defaults to a stretched
    // anchor, so children that live inside a VLG/HLG must be re-anchored after creation.
    static void NormalizeVerticalChild(GameObject go, float preferredHeight)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0, preferredHeight);
        var le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        le.minHeight = preferredHeight;
        le.preferredHeight = preferredHeight;
        le.flexibleHeight = 0;
    }

    static void NormalizeHorizontalChild(GameObject go, float preferredWidth, float flexibleWidth = 0)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(preferredWidth, 0);
        var le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        if (preferredWidth > 0)
        {
            le.minWidth = preferredWidth;
            le.preferredWidth = preferredWidth;
        }
        le.flexibleWidth = flexibleWidth;
    }

    static Text CreateText(Transform parent, string name, string defaultText, int fontSize, TextAnchor alignment)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = defaultText;
        t.font = font;
        t.fontSize = fontSize;
        t.alignment = alignment;
        t.color = Color.white;
        t.raycastTarget = false;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return t;
    }
}
