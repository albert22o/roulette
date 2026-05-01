using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DeadlyRoulette;

/// <summary>
/// Editor utility: builds the Post-Boss UI canvas (if missing) and wires every
/// SerializedProperty on PostBossManager / PostBossUI / RouletteUI.postBossUI.
/// Idempotent — safe to re-run.
/// Run via menu Tools > Build &amp; Wire Post-Boss UI.
/// </summary>
public class PostBossUIWirer
{
    [MenuItem("Tools/Build && Wire Post-Boss UI")]
    public static void BuildAndWire()
    {
        // --- 1. Ensure canvas exists ---
        var canvasGo = GameObject.Find("PostBossCanvas");
        if (canvasGo == null)
        {
            PostBossUIBuilder.Build();
            canvasGo = GameObject.Find("PostBossCanvas");
        }
        if (canvasGo == null)
        {
            Debug.LogError("PostBossCanvas could not be created.");
            return;
        }

        var mgr = canvasGo.GetComponent<PostBossManager>();
        var ui  = canvasGo.GetComponent<PostBossUI>();
        if (mgr == null || ui == null)
        {
            Debug.LogError("PostBossManager or PostBossUI component missing on PostBossCanvas.");
            return;
        }

        // --- 2. Find existing scene refs ---
        var rouletteGame = Object.FindFirstObjectByType<RouletteGame>();
        var shopManager  = Object.FindFirstObjectByType<ShopManager>();
        var rouletteUI   = Object.FindFirstObjectByType<RouletteUI>();
        if (rouletteGame == null || shopManager == null)
        {
            Debug.LogError("RouletteGame or ShopManager not found in scene.");
            return;
        }

        // --- 3. Wire PostBossManager ---
        var mgrSO = new SerializedObject(mgr);
        mgrSO.FindProperty("game").objectReferenceValue = rouletteGame;
        mgrSO.FindProperty("shop").objectReferenceValue = shopManager;
        mgrSO.ApplyModifiedProperties();

        // --- 4. Wire PostBossUI ---
        var panel = canvasGo.transform.Find("PostBossPanel");
        if (panel == null)
        {
            Debug.LogError("PostBossPanel not found under PostBossCanvas.");
            return;
        }

        var uiSO = new SerializedObject(ui);
        uiSO.FindProperty("mgr").objectReferenceValue  = mgr;
        uiSO.FindProperty("game").objectReferenceValue = rouletteGame;
        uiSO.FindProperty("panel").objectReferenceValue = panel.gameObject;

        var titleT = panel.Find("Header/TitleText");
        uiSO.FindProperty("titleText").objectReferenceValue = titleT != null ? titleT.GetComponent<Text>() : null;
        var goldT = panel.Find("Header/GoldText");
        uiSO.FindProperty("goldText").objectReferenceValue = goldT != null ? goldT.GetComponent<Text>() : null;

        // Slot UI array (5 slots)
        var slotsRow = panel.Find("SlotsRow");
        var slotsProp = uiSO.FindProperty("slotUI");
        slotsProp.arraySize = PostBossManager.TotalSlots;
        for (int i = 0; i < PostBossManager.TotalSlots; i++)
        {
            var slotT = slotsRow != null ? slotsRow.Find("Slot" + i) : null;
            var elem = slotsProp.GetArrayElementAtIndex(i);
            WireSlotElement(elem, slotT);
        }

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

        // Continue button
        var contBtn = panel.Find("ContinueButton");
        uiSO.FindProperty("continueBtn").objectReferenceValue = contBtn != null ? contBtn.GetComponent<Button>() : null;

        uiSO.ApplyModifiedProperties();

        // --- 5. Hook PostBossUI into RouletteUI ---
        if (rouletteUI != null)
        {
            var rouletteSO = new SerializedObject(rouletteUI);
            var prop = rouletteSO.FindProperty("postBossUI");
            if (prop != null)
            {
                prop.objectReferenceValue = ui;
                rouletteSO.ApplyModifiedProperties();
                EditorUtility.SetDirty(rouletteUI);
            }
            else
            {
                Debug.LogWarning("RouletteUI.postBossUI property not found — script may not be recompiled yet.");
            }
        }
        else
        {
            Debug.LogWarning("RouletteUI not found in scene; postBossUI link skipped.");
        }

        EditorUtility.SetDirty(mgr);
        EditorUtility.SetDirty(ui);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvasGo.scene);

        Debug.Log("Post-Boss UI built and wired successfully.");
    }

    static void WireSlotElement(SerializedProperty elem, Transform slotT)
    {
        if (slotT == null)
        {
            elem.FindPropertyRelative("root").objectReferenceValue = null;
            return;
        }
        elem.FindPropertyRelative("root").objectReferenceValue = slotT.gameObject;

        var nameT = slotT.Find("NameText");
        elem.FindPropertyRelative("nameText").objectReferenceValue = nameT != null ? nameT.GetComponent<Text>() : null;

        var descT = slotT.Find("DescText");
        elem.FindPropertyRelative("descText").objectReferenceValue = descT != null ? descT.GetComponent<Text>() : null;

        var kindT = slotT.Find("KindText");
        elem.FindPropertyRelative("kindText").objectReferenceValue = kindT != null ? kindT.GetComponent<Text>() : null;

        var claimBtn = slotT.Find("ClaimButton");
        elem.FindPropertyRelative("claimButton").objectReferenceValue = claimBtn != null ? claimBtn.GetComponent<Button>() : null;

        var claimedLbl = slotT.Find("ClaimedLabel");
        elem.FindPropertyRelative("claimedLabel").objectReferenceValue = claimedLbl != null ? claimedLbl.GetComponent<Text>() : null;
    }
}
