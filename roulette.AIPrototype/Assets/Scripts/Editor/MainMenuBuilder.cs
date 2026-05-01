using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;

/// <summary>
/// Editor utility: builds the Main Menu scene UI hierarchy.
/// Run via menu Tools > Build Main Menu UI in an open MainMenu scene.
/// </summary>
public class MainMenuBuilder
{
    static Font font;
    static readonly Color BG_BACKGROUND = new Color(0.05f, 0.05f, 0.08f, 1f);
    static readonly Color BG_BUTTON     = new Color(0.20f, 0.45f, 0.70f, 1f);
    static readonly Color BG_BUTTON_QUIT = new Color(0.45f, 0.20f, 0.20f, 1f);
    static readonly Color TEXT_TITLE    = new Color(0.95f, 0.85f, 0.30f, 1f);
    static readonly Color TEXT_BTN      = Color.white;

    [MenuItem("Tools/Build Main Menu UI")]
    public static void Build()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Find or create canvas
        var existingCanvas = Object.FindObjectOfType<Canvas>();
        Transform root;
        GameObject canvasGo;
        if (existingCanvas != null)
        {
            canvasGo = existingCanvas.gameObject;
            root = existingCanvas.transform;
        }
        else
        {
            canvasGo = new GameObject("MenuCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGo, "Build Main Menu UI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();
            root = canvasGo.transform;
        }

        // EventSystem (needed for button clicks)
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(es, "Build Main Menu UI");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // MainMenu controller GameObject
        var menuGo = GameObject.Find("MainMenu");
        if (menuGo == null)
        {
            menuGo = new GameObject("MainMenu");
            Undo.RegisterCreatedObjectUndo(menuGo, "Build Main Menu UI");
        }
        var menuUI = menuGo.GetComponent<DeadlyRoulette.MainMenuUI>();
        if (menuUI == null) menuUI = menuGo.AddComponent<DeadlyRoulette.MainMenuUI>();
        menuUI.gameSceneName = "SampleScene";

        // Background full-screen
        var bg = CreatePanel(root, "Background", BG_BACKGROUND);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Title text
        var titleGo = CreateText(root, "Title", "DEADLY ROULETTE", 96, TEXT_TITLE, TextAnchor.MiddleCenter);
        var titleRT = titleGo.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0, -180);
        titleRT.sizeDelta = new Vector2(1400, 160);
        var titleTxt = titleGo.GetComponent<Text>();
        titleTxt.fontStyle = FontStyle.Bold;

        // Subtitle
        var subGo = CreateText(root, "Subtitle", "A Roguelike Roulette", 36, new Color(0.8f, 0.8f, 0.85f, 1f), TextAnchor.MiddleCenter);
        var subRT = subGo.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.5f, 1f);
        subRT.anchorMax = new Vector2(0.5f, 1f);
        subRT.pivot = new Vector2(0.5f, 1f);
        subRT.anchoredPosition = new Vector2(0, -340);
        subRT.sizeDelta = new Vector2(1200, 60);
        subGo.GetComponent<Text>().fontStyle = FontStyle.Italic;

        // Start button
        var startBtnGo = CreateButton(root, "StartButton", "START GAME", BG_BUTTON);
        var startRT = startBtnGo.GetComponent<RectTransform>();
        startRT.anchorMin = new Vector2(0.5f, 0.5f);
        startRT.anchorMax = new Vector2(0.5f, 0.5f);
        startRT.pivot = new Vector2(0.5f, 0.5f);
        startRT.anchoredPosition = new Vector2(0, -40);
        startRT.sizeDelta = new Vector2(420, 100);

        var startBtn = startBtnGo.GetComponent<Button>();
        // Persistent listener so it survives scene save
        UnityEventTools.AddPersistentListener(startBtn.onClick, new UnityAction(menuUI.StartGame));

        // Quit button
        var quitBtnGo = CreateButton(root, "QuitButton", "QUIT", BG_BUTTON_QUIT);
        var quitRT = quitBtnGo.GetComponent<RectTransform>();
        quitRT.anchorMin = new Vector2(0.5f, 0.5f);
        quitRT.anchorMax = new Vector2(0.5f, 0.5f);
        quitRT.pivot = new Vector2(0.5f, 0.5f);
        quitRT.anchoredPosition = new Vector2(0, -180);
        quitRT.sizeDelta = new Vector2(420, 90);

        var quitBtn = quitBtnGo.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(quitBtn.onClick, new UnityAction(menuUI.QuitGame));

        // Footer hint
        var hintGo = CreateText(root, "Hint", "Click START to begin a new run", 28, new Color(0.6f, 0.6f, 0.65f, 1f), TextAnchor.MiddleCenter);
        var hintRT = hintGo.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(0.5f, 0f);
        hintRT.anchorMax = new Vector2(0.5f, 0f);
        hintRT.pivot = new Vector2(0.5f, 0f);
        hintRT.anchoredPosition = new Vector2(0, 60);
        hintRT.sizeDelta = new Vector2(1200, 50);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[MainMenuBuilder] Main Menu UI created. Save scene to persist.");
    }

    // ----- Helpers -----

    static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, "Build Main Menu UI");
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    static GameObject CreateText(Transform parent, string name, string content, int size, Color color, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        Undo.RegisterCreatedObjectUndo(go, "Build Main Menu UI");
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = content;
        t.font = font;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Color bg)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        Undo.RegisterCreatedObjectUndo(go, "Build Main Menu UI");
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bg;
        var btn = go.GetComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        btn.colors = colors;

        var labelGo = CreateText(go.transform, "Label", label, 48, TEXT_BTN, TextAnchor.MiddleCenter);
        labelGo.GetComponent<Text>().fontStyle = FontStyle.Bold;
        var lrt = labelGo.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;
        return go;
    }
}
