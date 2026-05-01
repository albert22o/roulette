using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor utility: creates the full Shop UI hierarchy in the active scene.
/// Run via menu Tools > Build Shop UI. Safe to delete after use.
/// </summary>
public class ShopUIBuilder
{
    static Font font;
    static readonly Color BG_DARK   = new Color(0.12f, 0.12f, 0.15f, 0.95f);
    static readonly Color BG_SECTION = new Color(0.18f, 0.18f, 0.22f, 1f);
    static readonly Color BG_SLOT   = new Color(0.25f, 0.25f, 0.30f, 1f);
    static readonly Color BG_POPUP  = new Color(0.10f, 0.10f, 0.13f, 0.98f);
    static readonly Color BTN_BUY   = new Color(0.2f, 0.6f, 0.3f, 1f);
    static readonly Color BTN_SELL  = new Color(0.7f, 0.25f, 0.25f, 1f);
    static readonly Color BTN_CLOSE = new Color(0.5f, 0.5f, 0.5f, 1f);
    static readonly Color TXT_GOLD  = new Color(1f, 0.85f, 0.2f, 1f);

    [MenuItem("Tools/Build Shop UI")]
    public static void Build()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // --- Canvas ---
        var canvasGo = new GameObject("ShopCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGo, "Build Shop UI");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // --- Shop Panel (full screen) ---
        var panel = CreatePanel(canvasGo.transform, "ShopPanel", BG_DARK, stretch: true);
        panel.SetActive(false); // starts hidden

        // --- Content area — stretched with bottom margin for the CloseButton ---
        var content = CreatePanel(panel.transform, "Content", Color.clear, stretch: true);
        var contentRT = content.GetComponent<RectTransform>();
        contentRT.offsetMin = new Vector2(40, 80);  // leave room for CloseButton (45 tall + 15 margin)
        contentRT.offsetMax = new Vector2(-40, -20);
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // --- Header ---
        var header = CreatePanel(content.transform, "Header", Color.clear, height: 50);
        var headerLayout = header.AddComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 20;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        headerLayout.childForceExpandHeight = true;
        CreateText(header.transform, "TitleText", "SHOP", 32, Color.white, width: 200);
        var goldTxt = CreateText(header.transform, "GoldText", "Gold: 0", 28, TXT_GOLD, width: 300);

        // --- Random Pool Section (5 slots in one row) ---
        var rpSection = CreateSection(content.transform, "RandomPoolSection", "RANDOM POOL");
        var rpRow = CreateRow(rpSection.transform, "Slots", 5);
        for (int i = 0; i < 5; i++)
            CreateShopSlot(rpRow.transform, "RandomSlot" + i);

        // --- Food Section ---
        var foodSection = CreateSection(content.transform, "FoodSection", "FOOD");
        var foodRow = CreateRow(foodSection.transform, "Slots", 1);
        var foodSlotGo = CreateShopSlot(foodRow.transform, "FoodSlot");

        // --- Other Items Section (2 ball tickets + always-available base money upgrade) ---
        var otherSection = CreateSection(content.transform, "OtherSection", "OTHER ITEMS");
        var otherRow = CreateRow(otherSection.transform, "Slots", 3);
        var otherSlot0 = CreateShopSlot(otherRow.transform, "OtherSlot0");
        var otherSlot1 = CreateShopSlot(otherRow.transform, "OtherSlot1");
        CreateBaseMoneySlot(otherRow.transform, "BaseMoneyUpgrade");

        // --- Inventory Section ---
        var invSection = CreateSection(content.transform, "InventorySection", "YOUR INVENTORY");
        var invRow = invSection.AddComponent<HorizontalLayoutGroup>() != null ? invSection : invSection;
        // Remove existing VLG, add HLG for side-by-side
        var existingVLG = invSection.GetComponent<VerticalLayoutGroup>();
        if (existingVLG) Object.DestroyImmediate(existingVLG);
        var invHLG = invSection.AddComponent<HorizontalLayoutGroup>();
        invHLG.spacing = 20;
        invHLG.childControlWidth = true;
        invHLG.childControlHeight = true;
        invHLG.childForceExpandWidth = true;
        invHLG.childForceExpandHeight = false;
        invHLG.childAlignment = TextAnchor.UpperCenter;

        // Section needs a preferred height since we removed the Content ContentSizeFitter
        var invLE = invSection.AddComponent<LayoutElement>();
        invLE.preferredHeight = 290;

        // Chips column
        var chipsCol = CreatePanel(invSection.transform, "ChipsColumn", Color.clear, height: 260);
        var chipsVLG = chipsCol.AddComponent<VerticalLayoutGroup>();
        chipsVLG.spacing = 4;
        chipsVLG.childControlWidth = true;
        chipsVLG.childControlHeight = true;
        chipsVLG.childForceExpandWidth = true;
        chipsVLG.childForceExpandHeight = false;
        CreateText(chipsCol.transform, "ChipsHeader", "Chips", 20, Color.white, height: 30);
        var chipEntries = new GameObject[10];
        for (int i = 0; i < 10; i++)
            chipEntries[i] = CreateInventoryEntry(chipsCol.transform, "ChipEntry" + i, BTN_SELL);

        // Balls column
        var ballsCol = CreatePanel(invSection.transform, "BallsColumn", Color.clear, height: 260);
        var ballsVLG = ballsCol.AddComponent<VerticalLayoutGroup>();
        ballsVLG.spacing = 4;
        ballsVLG.childControlWidth = true;
        ballsVLG.childControlHeight = true;
        ballsVLG.childForceExpandWidth = true;
        ballsVLG.childForceExpandHeight = false;
        CreateText(ballsCol.transform, "BallsHeader", "Balls", 20, Color.white, height: 30);
        var ballEntries = new GameObject[6];
        for (int i = 0; i < 6; i++)
            ballEntries[i] = CreateInventoryEntry(ballsCol.transform, "BallEntry" + i, BTN_SELL);

        // --- Close Button ---
        var closeBtnGo = CreateButton(panel.transform, "CloseButton", "CLOSE SHOP", BTN_CLOSE, 200, 45);
        var closeBtnRT = closeBtnGo.GetComponent<RectTransform>();
        closeBtnRT.anchorMin = new Vector2(0.5f, 0);
        closeBtnRT.anchorMax = new Vector2(0.5f, 0);
        closeBtnRT.pivot = new Vector2(0.5f, 0);
        closeBtnRT.anchoredPosition = new Vector2(0, 15);

        // ================================================================
        //  POPUPS
        // ================================================================

        // --- Number Selection Popup ---
        var numPopup = CreatePopup(panel.transform, "NumberPopup", 800, 500);
        var numContent = numPopup.transform.Find("PopupContent");
        var numPrompt = CreateText(numContent, "PromptText", "Select numbers:", 22, Color.white, height: 35);
        var numGrid = CreatePanel(numContent, "NumberGrid", Color.clear, height: 320);
        var gridLayout = numGrid.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(65, 45);
        gridLayout.spacing = new Vector2(4, 4);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 10;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        var numButtons = new GameObject[37];
        for (int i = 0; i <= 36; i++)
            numButtons[i] = CreateButton(numGrid.transform, "Num" + i, i.ToString(), NORMAL_BTN_COLOR(), 0, 0);

        var numConfirm = CreateButton(numContent, "ConfirmButton", "CONFIRM", BTN_BUY, 180, 40);

        // --- Group Selection Popup ---
        var grpPopup = CreatePopup(panel.transform, "GroupPopup", 600, 280);
        var grpContent = grpPopup.transform.Find("PopupContent");
        var grpPrompt = CreateText(grpContent, "GroupPromptText", "Select a group:", 22, Color.white, height: 35);
        var grpRow2 = CreatePanel(grpContent, "GroupRow", Color.clear, height: 60);
        var grpHLG = grpRow2.AddComponent<HorizontalLayoutGroup>();
        grpHLG.spacing = 10;
        grpHLG.childAlignment = TextAnchor.MiddleCenter;
        grpHLG.childControlWidth = true;
        grpHLG.childControlHeight = true;
        grpHLG.childForceExpandWidth = true;

        var grpButtons = new GameObject[6];
        string[] grpLabels = { "1st", "2nd", "3rd", "4th", "5th", "6th" };
        for (int i = 0; i < 6; i++)
            grpButtons[i] = CreateButton(grpRow2.transform, "Group" + (i + 1), grpLabels[i], NORMAL_BTN_COLOR(), 80, 50);

        var grpConfirm = CreateButton(grpContent, "ConfirmButton", "CONFIRM", BTN_BUY, 180, 40);

        // --- Ball Ticket Popup ---
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
        var sellBtn2 = CreateButton(ballBtnRow.transform, "SellButton", "SELL (3g)", BTN_SELL, 150, 40);

        // ================================================================
        //  ADD COMPONENTS
        // ================================================================

        var shopMgr = canvasGo.AddComponent<DeadlyRoulette.ShopManager>();
        var shopUI = canvasGo.AddComponent<DeadlyRoulette.ShopUI>();

        Debug.Log("Shop UI hierarchy built successfully! Assign RouletteGame reference in Inspector, then save scene.");
        Selection.activeGameObject = canvasGo;
    }

    // ==============================================================
    //  Factory helpers
    // ==============================================================

    static Color NORMAL_BTN_COLOR() => new Color(0.35f, 0.35f, 0.4f, 1f);

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
        t.alignment = TextAnchor.MiddleLeft;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
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

    static GameObject CreateSection(Transform parent, string name, string title)
    {
        // Section needs a LayoutElement since Content no longer uses ContentSizeFitter.
        // Height = title(28) + row(130) + padding(16) + spacing(8) = 182
        var section = CreatePanel(parent, name, BG_SECTION, height: 182);
        var vlg = section.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.padding = new RectOffset(12, 12, 8, 8);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateText(section.transform, "Title", title, 20, Color.white, height: 28);
        return section;
    }

    static GameObject CreateRow(Transform parent, string name, int slots)
    {
        var row = CreatePanel(parent, name, Color.clear, height: 130);
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;
        return row;
    }

    static GameObject CreateShopSlot(Transform parent, string name)
    {
        var slot = CreatePanel(parent, name, BG_SLOT);
        var vlg = slot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.padding = new RectOffset(8, 8, 6, 6);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateText(slot.transform, "NameText", "Item Name", 18, Color.white, height: 24);
        CreateText(slot.transform, "DescText", "Description", 14, Color.gray, height: 48);
        CreateText(slot.transform, "RarityText", "Common", 14, new Color(0.6f, 0.8f, 1f), height: 20);

        var costRow = CreatePanel(slot.transform, "CostRow", Color.clear, height: 30);
        var costHLG = costRow.AddComponent<HorizontalLayoutGroup>();
        costHLG.spacing = 8;
        costHLG.childAlignment = TextAnchor.MiddleLeft;
        costHLG.childControlWidth = true;
        costHLG.childControlHeight = true;
        costHLG.childForceExpandWidth = false;
        costHLG.childForceExpandHeight = true;
        CreateText(costRow.transform, "CostText", "4", 18, TXT_GOLD, width: 50);
        CreateButton(costRow.transform, "BuyButton", "BUY", BTN_BUY, 80, 28);

        return slot;
    }

    static GameObject CreateBaseMoneySlot(Transform parent, string name)
    {
        // Same visual shape as CreateShopSlot but with a simpler static label and no rarity.
        var slot = CreatePanel(parent, name, BG_SLOT);
        var vlg = slot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.padding = new RectOffset(8, 8, 6, 6);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateText(slot.transform, "NameText", "Base Money +100", 18, Color.white, height: 24);
        CreateText(slot.transform, "DescText", "Permanently increase base bankroll.", 14, Color.gray, height: 48);
        CreateText(slot.transform, "Spacer", "", 14, Color.clear, height: 20);

        var costRow = CreatePanel(slot.transform, "CostRow", Color.clear, height: 30);
        var costHLG = costRow.AddComponent<HorizontalLayoutGroup>();
        costHLG.spacing = 8;
        costHLG.childAlignment = TextAnchor.MiddleLeft;
        costHLG.childControlWidth = true;
        costHLG.childControlHeight = true;
        costHLG.childForceExpandWidth = false;
        costHLG.childForceExpandHeight = true;
        CreateText(costRow.transform, "CostText", "1", 18, TXT_GOLD, width: 50);
        CreateButton(costRow.transform, "BuyButton", "BUY", BTN_BUY, 80, 28);

        return slot;
    }

    static GameObject CreateInventoryEntry(Transform parent, string name, Color sellColor)
    {
        var entry = CreatePanel(parent, name, new Color(0.2f, 0.2f, 0.24f, 1f), height: 30);
        var hlg = entry.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6;
        hlg.padding = new RectOffset(6, 6, 2, 2);
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment = TextAnchor.MiddleLeft;

        var nameTxt = CreateText(entry.transform, "NameText", "---", 14, Color.white, width: 160);
        CreateButton(entry.transform, "SellButton", "Sell", sellColor, 50, 24);
        CreateText(entry.transform, "SellPriceText", "1", 14, TXT_GOLD, width: 30);

        return entry;
    }

    static GameObject CreatePopup(Transform parent, string name, float width, float height)
    {
        // Full screen dim overlay
        var overlay = CreatePanel(parent, name, new Color(0, 0, 0, 0.6f), stretch: true);
        overlay.SetActive(false);

        // Centered popup box
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
