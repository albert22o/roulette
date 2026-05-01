using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DeadlyRoulette;

/// <summary>
/// Editor utility: creates the Post-Boss Upgrade UI hierarchy in the active scene.
/// Run via menu Tools > Build Post-Boss UI. Wires PostBossManager + PostBossUI components.
/// </summary>
public class PostBossUIBuilder
{
    static Font font;
    static readonly Color BG_DARK   = new Color(0.10f, 0.08f, 0.14f, 0.97f);
    static readonly Color BG_SLOT   = new Color(0.22f, 0.20f, 0.28f, 1f);
    static readonly Color BTN_CLAIM = new Color(0.70f, 0.55f, 0.20f, 1f); // gold-ish
    static readonly Color BTN_BUY   = new Color(0.20f, 0.60f, 0.30f, 1f);
    static readonly Color BTN_SELL  = new Color(0.70f, 0.25f, 0.25f, 1f);
    static readonly Color BTN_CONT  = new Color(0.30f, 0.55f, 0.85f, 1f);
    static readonly Color TXT_GOLD  = new Color(1f, 0.85f, 0.20f, 1f);
    static readonly Color BG_POPUP  = new Color(0.10f, 0.10f, 0.13f, 0.98f);

    [MenuItem("Tools/Build Post-Boss UI")]
    public static void Build()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvasGo = new GameObject("PostBossCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGo, "Build Post-Boss UI");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 12; // above shop
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = CreatePanel(canvasGo.transform, "PostBossPanel", BG_DARK, stretch: true);
        panel.SetActive(false);

        // --- Header ---
        var header = CreatePanel(panel.transform, "Header", Color.clear, height: 80);
        var headerRT = header.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(1, 1);
        headerRT.pivot     = new Vector2(0.5f, 1);
        headerRT.anchoredPosition = new Vector2(0, -20);
        headerRT.sizeDelta = new Vector2(-80, 80);
        var headerHLG = header.AddComponent<HorizontalLayoutGroup>();
        headerHLG.spacing = 20;
        headerHLG.padding = new RectOffset(20, 20, 0, 0);
        headerHLG.childAlignment = TextAnchor.MiddleCenter;
        headerHLG.childControlWidth = true;
        headerHLG.childControlHeight = true;
        headerHLG.childForceExpandWidth = false;
        headerHLG.childForceExpandHeight = true;

        CreateText(header.transform, "TitleText",
            "BOSS DEFEATED  --  CHOOSE YOUR REWARDS", 32, Color.white, width: 800);
        CreateText(header.transform, "GoldText", "Gold: 0", 28, TXT_GOLD, width: 250);

        // --- Slot Row (5 slots) ---
        var slotsRow = CreatePanel(panel.transform, "SlotsRow", Color.clear);
        var slotsRT = slotsRow.GetComponent<RectTransform>();
        slotsRT.anchorMin = new Vector2(0.5f, 0.5f);
        slotsRT.anchorMax = new Vector2(0.5f, 0.5f);
        slotsRT.pivot     = new Vector2(0.5f, 0.5f);
        slotsRT.sizeDelta = new Vector2(1700, 380);
        var slotsHLG = slotsRow.AddComponent<HorizontalLayoutGroup>();
        slotsHLG.spacing = 18;
        slotsHLG.childAlignment = TextAnchor.MiddleCenter;
        slotsHLG.childControlWidth = true;
        slotsHLG.childControlHeight = true;
        slotsHLG.childForceExpandWidth = true;
        slotsHLG.childForceExpandHeight = true;

        var slotGOs = new GameObject[PostBossManager.TotalSlots];
        for (int i = 0; i < PostBossManager.TotalSlots; i++)
            slotGOs[i] = CreateSlot(slotsRow.transform, "Slot" + i);

        // --- Continue button ---
        var contBtnGo = CreateButton(panel.transform, "ContinueButton",
            "CONTINUE TO SHOP", BTN_CONT, 320, 60);
        var contRT = contBtnGo.GetComponent<RectTransform>();
        contRT.anchorMin = new Vector2(0.5f, 0);
        contRT.anchorMax = new Vector2(0.5f, 0);
        contRT.pivot     = new Vector2(0.5f, 0);
        contRT.anchoredPosition = new Vector2(0, 30);

        // --- Ball popup ---
        var ballPopupGo = CreatePopup(panel.transform, "BallPopup", 500, 300);
        var ballContent = ballPopupGo.transform.Find("PopupContent");
        var ballName = CreateText(ballContent, "BallName", "Ball Name", 26, Color.white, height: 35);
        var ballDesc = CreateText(ballContent, "BallDesc", "Description", 18, Color.gray, height: 60);
        var ballBtnRow = CreatePanel(ballContent, "ButtonRow", Color.clear, height: 50);
        var ballBtnHLG = ballBtnRow.AddComponent<HorizontalLayoutGroup>();
        ballBtnHLG.spacing = 20;
        ballBtnHLG.childAlignment = TextAnchor.MiddleCenter;
        ballBtnHLG.childControlWidth = true;
        ballBtnHLG.childControlHeight = true;
        ballBtnHLG.childForceExpandWidth = true;
        var acceptBtn = CreateButton(ballBtnRow.transform, "AcceptButton", "KEEP", BTN_BUY, 150, 40);
        var sellBtn = CreateButton(ballBtnRow.transform, "SellButton", "SELL (3g)", BTN_SELL, 150, 40);

        // --- Components ---
        var mgr = canvasGo.AddComponent<PostBossManager>();
        var ui = canvasGo.AddComponent<PostBossUI>();

        Debug.Log("Post-Boss UI hierarchy built! Assign 'game' / 'shop' on PostBossManager and " +
                  "PostBossUI fields in the Inspector, then save scene.");
        Selection.activeGameObject = canvasGo;
    }

    static GameObject CreateSlot(Transform parent, string name)
    {
        var slot = CreatePanel(parent, name, BG_SLOT);
        var vlg = slot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateText(slot.transform, "KindText",     "Reward",  14, new Color(0.7f, 0.7f, 0.85f), height: 18);
        CreateText(slot.transform, "NameText",     "Name",    20, Color.white, height: 28);
        CreateText(slot.transform, "DescText",     "Desc",    14, Color.gray, height: 130);
        CreateButton(slot.transform, "ClaimButton","CLAIM",   BTN_CLAIM, 0, 40);
        CreateText(slot.transform, "ClaimedLabel", "CLAIMED", 16, new Color(0.4f, 1f, 0.4f), height: 30);

        return slot;
    }

    // ---- shared helpers (kept local to avoid coupling with ShopUIBuilder) ----

    static GameObject CreatePanel(Transform parent, string name, Color bg, bool stretch = false, float height = 0)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bg;
        if (bg.a < 0.01f) img.enabled = false;

        var rt = go.GetComponent<RectTransform>();
        if (stretch)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else if (height > 0)
        {
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.flexibleWidth = 1;
        }
        return go;
    }

    static GameObject CreateText(Transform parent, string name, string text, int size, Color color,
        float width = 0, float height = 0)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = text;
        t.font = font;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        if (width > 0 || height > 0)
        {
            var le = go.AddComponent<LayoutElement>();
            if (width > 0) le.preferredWidth = width;
            if (height > 0) le.preferredHeight = height;
        }
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Color bg,
        float width, float height)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bg;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.font = font;
        txt.fontSize = 16;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        var txtRT = txtGo.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        if (width > 0 || height > 0)
        {
            var le = go.AddComponent<LayoutElement>();
            if (width > 0) le.preferredWidth = width;
            if (height > 0) le.preferredHeight = height;
        }
        return go;
    }

    static GameObject CreatePopup(Transform parent, string name, float width, float height)
    {
        var overlay = CreatePanel(parent, name, new Color(0, 0, 0, 0.6f), stretch: true);
        overlay.SetActive(false);

        var box = CreatePanel(overlay.transform, "PopupContent", BG_POPUP);
        var boxRT = box.GetComponent<RectTransform>();
        boxRT.anchorMin = new Vector2(0.5f, 0.5f);
        boxRT.anchorMax = new Vector2(0.5f, 0.5f);
        boxRT.pivot = new Vector2(0.5f, 0.5f);
        boxRT.sizeDelta = new Vector2(width, height);

        var vlg = box.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.padding = new RectOffset(16, 16, 12, 12);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        return overlay;
    }
}
