using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DeadlyRoulette;

/// <summary>
/// Editor utility: wires all serialized references on ShopManager and ShopUI
/// to the existing scene hierarchy built by ShopUIBuilder.
/// Run via menu Tools > Wire Shop UI.
/// </summary>
public class ShopUIWirer
{
    [MenuItem("Tools/Wire Shop UI")]
    public static void Wire()
    {
        var shopCanvas = GameObject.Find("ShopCanvas");
        if (shopCanvas == null)
        {
            Debug.LogError("ShopCanvas not found in scene!");
            return;
        }

        var shopMgr = shopCanvas.GetComponent<ShopManager>();
        var shopUI = shopCanvas.GetComponent<ShopUI>();
        if (shopMgr == null || shopUI == null)
        {
            Debug.LogError("ShopManager or ShopUI component not found on ShopCanvas!");
            return;
        }

        // Find RouletteGame
        var gameObj = GameObject.Find("Game");
        var rouletteGame = gameObj != null ? gameObj.GetComponent<RouletteGame>() : null;
        if (rouletteGame == null)
        {
            Debug.LogError("RouletteGame not found on 'Game' object!");
            return;
        }

        // Helper: find inactive child by path
        Transform panel = shopCanvas.transform.Find("ShopPanel");
        if (panel == null)
        {
            Debug.LogError("ShopPanel not found!");
            return;
        }

        // ================================================================
        //  Wire ShopManager
        // ================================================================
        var mgrSO = new SerializedObject(shopMgr);
        mgrSO.FindProperty("game").objectReferenceValue = rouletteGame;
        mgrSO.ApplyModifiedProperties();

        // ================================================================
        //  Wire ShopUI
        // ================================================================
        var uiSO = new SerializedObject(shopUI);

        // Basic references
        uiSO.FindProperty("shop").objectReferenceValue = shopMgr;
        uiSO.FindProperty("game").objectReferenceValue = rouletteGame;
        uiSO.FindProperty("shopPanel").objectReferenceValue = panel.gameObject;

        // Gold text
        var goldText = panel.Find("Content/Header/GoldText");
        uiSO.FindProperty("goldText").objectReferenceValue = goldText != null ? goldText.GetComponent<Text>() : null;

        // Random pool UI (5 slots)
        WireShopSlotArray(uiSO, "randomPoolUI", panel, new[] {
            "Content/RandomPoolSection/Slots/RandomSlot0",
            "Content/RandomPoolSection/Slots/RandomSlot1",
            "Content/RandomPoolSection/Slots/RandomSlot2",
            "Content/RandomPoolSection/Slots/RandomSlot3",
            "Content/RandomPoolSection/Slots/RandomSlot4"
        });

        // Food UI (single slot)
        WireShopSlotSingle(uiSO, "foodUI", panel, "Content/FoodSection/Slots/FoodSlot");

        // Other UI (2 ball ticket slots)
        WireShopSlotArray(uiSO, "otherUI", panel, new[] {
            "Content/OtherSection/Slots/OtherSlot0",
            "Content/OtherSection/Slots/OtherSlot1"
        });

        // Base money upgrade slot
        var bmRoot = panel.Find("Content/OtherSection/Slots/BaseMoneyUpgrade");
        uiSO.FindProperty("baseMoneyRoot").objectReferenceValue = bmRoot != null ? bmRoot.gameObject : null;
        var bmName = bmRoot != null ? bmRoot.Find("NameText") : null;
        uiSO.FindProperty("baseMoneyNameText").objectReferenceValue = bmName != null ? bmName.GetComponent<Text>() : null;
        var bmDesc = bmRoot != null ? bmRoot.Find("DescText") : null;
        uiSO.FindProperty("baseMoneyDescText").objectReferenceValue = bmDesc != null ? bmDesc.GetComponent<Text>() : null;
        var bmCost = bmRoot != null ? bmRoot.Find("CostRow/CostText") : null;
        uiSO.FindProperty("baseMoneyCostText").objectReferenceValue = bmCost != null ? bmCost.GetComponent<Text>() : null;
        var bmBtn = bmRoot != null ? bmRoot.Find("CostRow/BuyButton") : null;
        uiSO.FindProperty("baseMoneyBuyBtn").objectReferenceValue = bmBtn != null ? bmBtn.GetComponent<Button>() : null;

        // Chip entries (10)
        WireInventoryEntryArray(uiSO, "chipEntries", panel, "Content/InventorySection/ChipsColumn/ChipEntry", 10);

        // Ball entries (6)
        WireInventoryEntryArray(uiSO, "ballEntries", panel, "Content/InventorySection/BallsColumn/BallEntry", 6);

        // Number popup
        var numPopup = panel.Find("NumberPopup");
        uiSO.FindProperty("numberPopup").objectReferenceValue = numPopup != null ? numPopup.gameObject : null;

        var numPrompt = panel.Find("NumberPopup/PopupContent/PromptText");
        uiSO.FindProperty("numberPromptText").objectReferenceValue = numPrompt != null ? numPrompt.GetComponent<Text>() : null;

        // Number buttons (0-36)
        var numGrid = panel.Find("NumberPopup/PopupContent/NumberGrid");
        var numBtnsProp = uiSO.FindProperty("numberButtons");
        numBtnsProp.arraySize = 37;
        for (int i = 0; i <= 36; i++)
        {
            var numBtn = numGrid != null ? numGrid.Find("Num" + i) : null;
            numBtnsProp.GetArrayElementAtIndex(i).objectReferenceValue = numBtn != null ? numBtn.GetComponent<Button>() : null;
        }

        var numConfirm = panel.Find("NumberPopup/PopupContent/ConfirmButton");
        uiSO.FindProperty("numberConfirmBtn").objectReferenceValue = numConfirm != null ? numConfirm.GetComponent<Button>() : null;

        // Group popup
        var grpPopup = panel.Find("GroupPopup");
        uiSO.FindProperty("groupPopup").objectReferenceValue = grpPopup != null ? grpPopup.gameObject : null;

        var grpPrompt = panel.Find("GroupPopup/PopupContent/GroupPromptText");
        uiSO.FindProperty("groupPromptText").objectReferenceValue = grpPrompt != null ? grpPrompt.GetComponent<Text>() : null;

        var grpRow = panel.Find("GroupPopup/PopupContent/GroupRow");
        var grpBtnsProp = uiSO.FindProperty("groupButtons");
        grpBtnsProp.arraySize = 6;
        for (int i = 0; i < 6; i++)
        {
            var grpBtn = grpRow != null ? grpRow.Find("Group" + (i + 1)) : null;
            grpBtnsProp.GetArrayElementAtIndex(i).objectReferenceValue = grpBtn != null ? grpBtn.GetComponent<Button>() : null;
        }

        var grpConfirm = panel.Find("GroupPopup/PopupContent/ConfirmButton");
        uiSO.FindProperty("groupConfirmBtn").objectReferenceValue = grpConfirm != null ? grpConfirm.GetComponent<Button>() : null;

        // Ball popup
        var ballPopup = panel.Find("BallPopup");
        uiSO.FindProperty("ballPopup").objectReferenceValue = ballPopup != null ? ballPopup.gameObject : null;

        var ballName = panel.Find("BallPopup/PopupContent/BallName");
        uiSO.FindProperty("ballNameText").objectReferenceValue = ballName != null ? ballName.GetComponent<Text>() : null;

        var ballDesc = panel.Find("BallPopup/PopupContent/BallDesc");
        uiSO.FindProperty("ballDescText").objectReferenceValue = ballDesc != null ? ballDesc.GetComponent<Text>() : null;

        var acceptBtn = panel.Find("BallPopup/PopupContent/ButtonRow/AcceptButton");
        uiSO.FindProperty("acceptBallBtn").objectReferenceValue = acceptBtn != null ? acceptBtn.GetComponent<Button>() : null;

        var sellBtn = panel.Find("BallPopup/PopupContent/ButtonRow/SellButton");
        uiSO.FindProperty("sellBallBtn").objectReferenceValue = sellBtn != null ? sellBtn.GetComponent<Button>() : null;

        // Close button
        var closeBtn = panel.Find("CloseButton");
        uiSO.FindProperty("closeBtn").objectReferenceValue = closeBtn != null ? closeBtn.GetComponent<Button>() : null;

        uiSO.ApplyModifiedProperties();

        EditorUtility.SetDirty(shopMgr);
        EditorUtility.SetDirty(shopUI);

        Debug.Log("Shop UI wiring complete! All references assigned.");
    }

    // ================================================================
    //  Helpers
    // ================================================================

    static void WireShopSlotArray(SerializedObject so, string propName, Transform root, string[] paths)
    {
        var prop = so.FindProperty(propName);
        prop.arraySize = paths.Length;

        for (int i = 0; i < paths.Length; i++)
        {
            var slotT = root.Find(paths[i]);
            if (slotT == null)
            {
                Debug.LogWarning($"WireShopSlotArray: '{paths[i]}' not found under '{root.name}'");
                continue;
            }

            var elem = prop.GetArrayElementAtIndex(i);
            WireShopSlotElement(elem, slotT);
        }
    }

    static void WireShopSlotSingle(SerializedObject so, string propName, Transform root, string path)
    {
        var slotT = root.Find(path);
        if (slotT == null)
        {
            Debug.LogWarning($"WireShopSlotSingle: '{path}' not found");
            return;
        }

        var prop = so.FindProperty(propName);
        WireShopSlotElement(prop, slotT);
    }

    static void WireShopSlotElement(SerializedProperty elem, Transform slotT)
    {
        elem.FindPropertyRelative("root").objectReferenceValue = slotT.gameObject;

        var nameT = slotT.Find("NameText");
        elem.FindPropertyRelative("nameText").objectReferenceValue = nameT != null ? nameT.GetComponent<Text>() : null;

        var descT = slotT.Find("DescText");
        elem.FindPropertyRelative("descText").objectReferenceValue = descT != null ? descT.GetComponent<Text>() : null;

        var costRow = slotT.Find("CostRow");
        var costT = costRow != null ? costRow.Find("CostText") : null;
        elem.FindPropertyRelative("costText").objectReferenceValue = costT != null ? costT.GetComponent<Text>() : null;

        var rarityT = slotT.Find("RarityText");
        elem.FindPropertyRelative("rarityText").objectReferenceValue = rarityT != null ? rarityT.GetComponent<Text>() : null;

        // Buy button: look in CostRow first, then SelectButton (booster cards)
        Button buyBtn = null;
        if (costRow != null)
        {
            var buyT = costRow.Find("BuyButton");
            if (buyT != null) buyBtn = buyT.GetComponent<Button>();
        }
        if (buyBtn == null)
        {
            var selT = slotT.Find("SelectButton");
            if (selT != null) buyBtn = selT.GetComponent<Button>();
        }
        elem.FindPropertyRelative("buyButton").objectReferenceValue = buyBtn;
    }

    static void WireInventoryEntryArray(SerializedObject so, string propName, Transform root, string pathPrefix, int count)
    {
        var prop = so.FindProperty(propName);
        prop.arraySize = count;

        for (int i = 0; i < count; i++)
        {
            var entryT = root.Find(pathPrefix + i);
            if (entryT == null)
            {
                Debug.LogWarning($"WireInventoryEntryArray: '{pathPrefix}{i}' not found");
                continue;
            }

            var elem = prop.GetArrayElementAtIndex(i);
            elem.FindPropertyRelative("root").objectReferenceValue = entryT.gameObject;

            var nameT = entryT.Find("NameText");
            elem.FindPropertyRelative("nameText").objectReferenceValue = nameT != null ? nameT.GetComponent<Text>() : null;

            var sellBtnT = entryT.Find("SellButton");
            elem.FindPropertyRelative("sellButton").objectReferenceValue = sellBtnT != null ? sellBtnT.GetComponent<Button>() : null;

            var sellPriceT = entryT.Find("SellPriceText");
            elem.FindPropertyRelative("sellPriceText").objectReferenceValue = sellPriceT != null ? sellPriceT.GetComponent<Text>() : null;
        }
    }
}
