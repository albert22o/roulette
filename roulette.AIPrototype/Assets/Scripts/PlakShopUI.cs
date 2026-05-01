using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeadlyRoulette
{
    /// <summary>
    /// Minimal programmatically-built UI for the boss-chip plak shop.
    /// Adds a "Spend Boss Chip" button near the shop's Continue button and
    /// handles the plak-pick / number-pick flow in its own overlay.
    /// </summary>
    public class PlakShopUI : MonoBehaviour
    {
        [SerializeField] PlakShop plakShop;
        [SerializeField] RouletteGame game;

        [Tooltip("Parent (usually ShopUI.shopPanel) that hosts the Spend Boss Chip button.")]
        [SerializeField] RectTransform shopPanel;

        Button spendButton;
        Text spendLabel;

        GameObject overlay;             // parent dimmer for plak selection / number picking
        GameObject offersPanel;
        Text offersTitle;
        List<Button> offerButtons = new List<Button>();
        List<Text> offerLabels = new List<Text>();

        GameObject pickerPanel;
        Text pickerPrompt;
        List<Button> numberBtns = new List<Button>();   // 0..36
        List<Text> numberLabels = new List<Text>();
        List<Image> numberImages = new List<Image>();
        List<Text> numberBadges = new List<Text>();     // small overlay: "±2", "+2", etc.
        Button pickerConfirm;
        Button pickerCancel;

        static readonly Color SELECTED = new Color(1f, 0.92f, 0.25f, 1f);
        static readonly Color DISABLED = new Color(0.25f, 0.25f, 0.25f, 1f);
        // Roulette base colors for number cells.
        static readonly Color C_RED    = new Color(0.70f, 0.15f, 0.15f, 1f);
        static readonly Color C_BLACK  = new Color(0.16f, 0.16f, 0.20f, 1f);
        static readonly Color C_GREEN  = new Color(0.15f, 0.50f, 0.25f, 1f);
        // Plak recolor shades.
        static readonly Color C_GRAY   = new Color(0.55f, 0.55f, 0.55f, 1f);
        static readonly Color C_PURPLE = new Color(0.55f, 0.20f, 0.70f, 1f);
        static readonly Color C_BLUE   = new Color(0.25f, 0.55f, 0.95f, 1f);
        static readonly Color C_WHITE  = new Color(0.95f, 0.95f, 0.95f, 1f);
        static readonly Color C_YELLOW = new Color(0.95f, 0.85f, 0.25f, 1f);
        static readonly Color C_ORANGE = new Color(0.95f, 0.55f, 0.15f, 1f);

        // Picker state
        PlakId pickerPlak;
        int picksNeeded;
        bool needsSource;               // Green plak: first pick is the source number
        int sourceNumber = -1;
        List<int> selected = new List<int>();

        void Awake()
        {
            if (plakShop == null) plakShop = GetComponent<PlakShop>();
            if (game == null) game = GetComponent<RouletteGame>();
        }

        void OnEnable()
        {
            if (plakShop != null) plakShop.OnChanged += Refresh;
            BuildUI();
            Refresh();
        }

        void OnDisable()
        {
            if (plakShop != null) plakShop.OnChanged -= Refresh;
        }

        void Update()
        {
            if (spendLabel != null && game != null && spendButton != null)
            {
                spendLabel.text = $"Spend Boss Chip ({game.inventory.bossChips})";
                spendButton.interactable = plakShop != null && plakShop.CanOpen();
            }
        }

        // ==============================================================
        //  UI construction
        // ==============================================================

        void BuildUI()
        {
            if (shopPanel == null) return;
            if (spendButton == null) BuildSpendButton();
            if (overlay == null) BuildOverlay();
        }

        void BuildSpendButton()
        {
            var go = new GameObject("SpendBossChipButton",
                typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(shopPanel, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(220f, -20f);
            rt.sizeDelta = new Vector2(200f, 48f);

            go.GetComponent<Image>().color = new Color(0.45f, 0.25f, 0.5f, 1f);

            spendButton = go.GetComponent<Button>();
            spendLabel = CreateChildText(go.transform, "Spend Boss Chip (0)", 16, TextAnchor.MiddleCenter);

            spendButton.onClick.AddListener(OnSpendClicked);
        }

        void BuildOverlay()
        {
            overlay = new GameObject("PlakOverlay", typeof(RectTransform), typeof(Image));
            overlay.transform.SetParent(shopPanel, false);
            var ort = (RectTransform)overlay.transform;
            ort.anchorMin = Vector2.zero;
            ort.anchorMax = Vector2.one;
            ort.offsetMin = Vector2.zero;
            ort.offsetMax = Vector2.zero;
            overlay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            overlay.SetActive(false);

            BuildOffersPanel();
            BuildPickerPanel();
        }

        void BuildOffersPanel()
        {
            offersPanel = new GameObject("OffersPanel", typeof(RectTransform), typeof(Image));
            offersPanel.transform.SetParent(overlay.transform, false);
            var rt = (RectTransform)offersPanel.transform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(640f, 420f);
            rt.anchoredPosition = Vector2.zero;
            offersPanel.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 1f);

            offersTitle = CreateChildText(offersPanel.transform, "Choose a plak", 22, TextAnchor.UpperCenter);
            ((RectTransform)offersTitle.transform).anchoredPosition = new Vector2(0f, -20f);

            for (int i = 0; i < PlakShop.OfferCount; i++)
            {
                int idx = i;
                var btnGo = new GameObject($"Offer{i}",
                    typeof(RectTransform), typeof(Image), typeof(Button));
                btnGo.transform.SetParent(offersPanel.transform, false);
                var brt = (RectTransform)btnGo.transform;
                brt.anchorMin = new Vector2(0.5f, 1f);
                brt.anchorMax = new Vector2(0.5f, 1f);
                brt.pivot = new Vector2(0.5f, 1f);
                brt.sizeDelta = new Vector2(560f, 90f);
                brt.anchoredPosition = new Vector2(0f, -60f - i * 100f);
                btnGo.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.32f, 1f);

                var btn = btnGo.GetComponent<Button>();
                btn.onClick.AddListener(() => OnOfferClicked(idx));
                offerButtons.Add(btn);

                var label = CreateChildText(btnGo.transform, "", 14, TextAnchor.MiddleLeft);
                ((RectTransform)label.transform).offsetMin = new Vector2(16f, 0f);
                ((RectTransform)label.transform).offsetMax = new Vector2(-16f, 0f);
                offerLabels.Add(label);
            }

            // Cancel button
            var cancelGo = new GameObject("Cancel",
                typeof(RectTransform), typeof(Image), typeof(Button));
            cancelGo.transform.SetParent(offersPanel.transform, false);
            var crt = (RectTransform)cancelGo.transform;
            crt.anchorMin = new Vector2(1f, 0f);
            crt.anchorMax = new Vector2(1f, 0f);
            crt.pivot = new Vector2(1f, 0f);
            crt.sizeDelta = new Vector2(120f, 40f);
            crt.anchoredPosition = new Vector2(-16f, 16f);
            cancelGo.GetComponent<Image>().color = new Color(0.4f, 0.22f, 0.22f, 1f);
            CreateChildText(cancelGo.transform, "Cancel", 14, TextAnchor.MiddleCenter);
            // Closing after Open refunds the boss chip to avoid losing it accidentally.
            cancelGo.GetComponent<Button>().onClick.AddListener(OnCancelOffers);
        }

        void BuildPickerPanel()
        {
            pickerPanel = new GameObject("PickerPanel", typeof(RectTransform), typeof(Image));
            pickerPanel.transform.SetParent(overlay.transform, false);
            var rt = (RectTransform)pickerPanel.transform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(780f, 520f);
            rt.anchoredPosition = Vector2.zero;
            pickerPanel.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 1f);
            pickerPanel.SetActive(false);

            pickerPrompt = CreateChildText(pickerPanel.transform, "", 18, TextAnchor.UpperCenter);
            ((RectTransform)pickerPrompt.transform).anchoredPosition = new Vector2(0f, -16f);
            ((RectTransform)pickerPrompt.transform).sizeDelta = new Vector2(740f, 40f);

            // European roulette bet-table layout: 0 tall on the left, 1..36 in 3 rows × 12 columns.
            const float BTN_W = 52f, BTN_H = 44f, COL_W = 54f, ROW_H = 50f;
            const float GRID_START_X = -297f;
            const float GRID_TOP_Y = -90f;

            AddNumberBtn(0, new Vector2(GRID_START_X - COL_W, GRID_TOP_Y), new Vector2(BTN_W, ROW_H * 3f - 6f));

            for (int n = 1; n <= 36; n++)
            {
                int col = (n - 1) / 3;
                int row = 2 - ((n - 1) % 3);
                float x = GRID_START_X + col * COL_W;
                float y = GRID_TOP_Y - row * ROW_H;
                AddNumberBtn(n, new Vector2(x, y), new Vector2(BTN_W, BTN_H));
            }

            // Confirm
            var conGo = new GameObject("Confirm",
                typeof(RectTransform), typeof(Image), typeof(Button));
            conGo.transform.SetParent(pickerPanel.transform, false);
            var crt = (RectTransform)conGo.transform;
            crt.anchorMin = new Vector2(1f, 0f);
            crt.anchorMax = new Vector2(1f, 0f);
            crt.pivot = new Vector2(1f, 0f);
            crt.sizeDelta = new Vector2(140f, 44f);
            crt.anchoredPosition = new Vector2(-16f, 16f);
            conGo.GetComponent<Image>().color = new Color(0.25f, 0.5f, 0.3f, 1f);
            CreateChildText(conGo.transform, "Confirm", 14, TextAnchor.MiddleCenter);
            pickerConfirm = conGo.GetComponent<Button>();
            pickerConfirm.onClick.AddListener(OnPickerConfirm);

            // Cancel — aborts and returns to offers (boss chip stays spent).
            var cancelGo = new GameObject("Back",
                typeof(RectTransform), typeof(Image), typeof(Button));
            cancelGo.transform.SetParent(pickerPanel.transform, false);
            var canrt = (RectTransform)cancelGo.transform;
            canrt.anchorMin = new Vector2(0f, 0f);
            canrt.anchorMax = new Vector2(0f, 0f);
            canrt.pivot = new Vector2(0f, 0f);
            canrt.sizeDelta = new Vector2(120f, 40f);
            canrt.anchoredPosition = new Vector2(16f, 16f);
            cancelGo.GetComponent<Image>().color = new Color(0.4f, 0.22f, 0.22f, 1f);
            CreateChildText(cancelGo.transform, "Back", 14, TextAnchor.MiddleCenter);
            pickerCancel = cancelGo.GetComponent<Button>();
            pickerCancel.onClick.AddListener(OnPickerBack);
        }

        void AddNumberBtn(int n, Vector2 pos, Vector2 size)
        {
            var go = new GameObject($"N{n}",
                typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(pickerPanel.transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;

            var img = go.GetComponent<Image>();
            img.color = BaseRouletteColor(n);

            var label = CreateChildText(go.transform, n.ToString(), 18, TextAnchor.MiddleCenter);
            label.fontStyle = FontStyle.Bold;

            // Small badge overlay (±2, +2) anchored to bottom-right.
            var badgeGo = new GameObject("Badge", typeof(RectTransform), typeof(Text));
            badgeGo.transform.SetParent(go.transform, false);
            var badgeRt = (RectTransform)badgeGo.transform;
            badgeRt.anchorMin = new Vector2(1f, 0f);
            badgeRt.anchorMax = new Vector2(1f, 0f);
            badgeRt.pivot = new Vector2(1f, 0f);
            badgeRt.anchoredPosition = new Vector2(-2f, 2f);
            badgeRt.sizeDelta = new Vector2(32f, 16f);
            var badge = badgeGo.GetComponent<Text>();
            badge.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            badge.fontSize = 11;
            badge.color = Color.white;
            badge.alignment = TextAnchor.LowerRight;
            badge.text = "";

            var btn = go.GetComponent<Button>();
            int captured = n;
            btn.onClick.AddListener(() => OnNumberClicked(captured));
            // Neutralise Unity button tint so our Image.color stays true.
            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            btn.colors = cb;

            while (numberBtns.Count <= n)
            {
                numberBtns.Add(null);
                numberLabels.Add(null);
                numberImages.Add(null);
                numberBadges.Add(null);
            }
            numberBtns[n] = btn;
            numberLabels[n] = label;
            numberImages[n] = img;
            numberBadges[n] = badge;
        }

        static Color BaseRouletteColor(int n)
        {
            if (n == 0) return C_GREEN;
            return RouletteData.ColorOf(n) == BetColor.Red ? C_RED : C_BLACK;
        }

        static Color EffectiveColor(NumberColor nc, int n)
        {
            switch (nc)
            {
                case NumberColor.Red:    return C_RED;
                case NumberColor.Black:  return C_BLACK;
                case NumberColor.Gray:   return C_GRAY;
                case NumberColor.Purple: return C_PURPLE;
                case NumberColor.Blue:   return C_BLUE;
                case NumberColor.White:  return C_WHITE;
                case NumberColor.Yellow: return C_YELLOW;
                case NumberColor.Orange: return C_ORANGE;
                default: return BaseRouletteColor(n);
            }
        }

        static Text CreateChildText(Transform parent, string content, int fontSize, TextAnchor anchor)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = fontSize;
            t.alignment = anchor;
            t.color = Color.white;
            t.text = content;
            return t;
        }

        // ==============================================================
        //  Flow
        // ==============================================================

        void OnSpendClicked()
        {
            if (plakShop == null) return;
            if (!plakShop.Open()) return;
            ShowOffers();
        }

        void OnCancelOffers()
        {
            if (plakShop != null && plakShop.IsOpen)
            {
                // Refund the boss chip — the shop was never finalised.
                game.inventory.bossChips++;
                plakShop.Close();
            }
            HideOverlay();
        }

        void OnOfferClicked(int idx)
        {
            if (plakShop == null || idx < 0 || idx >= plakShop.currentOffers.Count) return;
            var plak = plakShop.currentOffers[idx];
            var def = PlakRegistry.Plaks[plak];

            if (plak == PlakId.Gray)
            {
                // No picks needed — apply immediately.
                plakShop.Apply(plak, null);
                HideOverlay();
                return;
            }

            pickerPlak = plak;
            picksNeeded = def.NumbersToPick;
            needsSource = def.PicksSourceFirst;
            sourceNumber = -1;
            selected.Clear();
            ShowPicker();
        }

        void ShowOffers()
        {
            if (overlay != null) overlay.SetActive(true);
            offersPanel.SetActive(true);
            pickerPanel.SetActive(false);
            for (int i = 0; i < offerButtons.Count; i++)
            {
                bool has = i < plakShop.currentOffers.Count;
                offerButtons[i].gameObject.SetActive(has);
                if (!has) continue;
                var def = PlakRegistry.Plaks[plakShop.currentOffers[i]];
                offerLabels[i].text = $"{def.Name}\n<size=12>{def.Description}</size>";
                offerLabels[i].supportRichText = true;
            }
        }

        void ShowPicker()
        {
            offersPanel.SetActive(false);
            pickerPanel.SetActive(true);
            UpdatePickerPrompt();
            UpdateNumberButtons();
        }

        void HideOverlay()
        {
            if (overlay != null) overlay.SetActive(false);
        }

        void UpdatePickerPrompt()
        {
            if (pickerPrompt == null) return;
            var def = PlakRegistry.Plaks[pickerPlak];
            if (needsSource && sourceNumber < 0)
                pickerPrompt.text = $"{def.Name}: pick the source number.";
            else
                pickerPrompt.text = $"{def.Name}: pick {picksNeeded} number(s). ({selected.Count}/{picksNeeded})";
        }

        void UpdateNumberButtons()
        {
            var def = PlakRegistry.Plaks[pickerPlak];
            var fx = new EffectProcessor(game.inventory);

            // Pre-compute plak badges (shown regardless of picker type so the player can see
            // existing modifications while choosing).
            var badgeText = new Dictionary<int, string>();
            var badgeColor = new Dictionary<int, Color>();
            foreach (var p in game.inventory.plaks)
            {
                if (p.TargetNumbers == null || p.TargetNumbers.Length == 0) continue;
                if (p.Id == PlakId.PlusMinus)
                {
                    badgeText[p.TargetNumbers[0]] = "\u00B12";
                    badgeColor[p.TargetNumbers[0]] = new Color(0.4f, 0.9f, 1f);
                }
                else if (p.Id == PlakId.Plus)
                {
                    badgeText[p.TargetNumbers[0]] = "x3";
                    badgeColor[p.TargetNumbers[0]] = new Color(1f, 0.85f, 0.3f);
                }
            }

            for (int n = 0; n <= 36; n++)
            {
                if (n >= numberBtns.Count || numberBtns[n] == null) continue;
                bool removed = fx.IsNumberRemoved(n);
                bool allowZero = pickerPlak != PlakId.Black && pickerPlak != PlakId.Red;

                bool allowed = !removed;
                if (n == 0 && !allowZero) allowed = false;

                // Color restriction (Red/Black plaks): only let the player pick the opposite color.
                if (allowed && def.PickColorRestriction != BetColor.None && n > 0)
                {
                    if (RouletteData.ColorOf(n) != def.PickColorRestriction) allowed = false;
                }

                // Green picker: during replace phase, exclude the already-chosen source.
                if (pickerPlak == PlakId.Green && !needsSource && sourceNumber == n)
                    allowed = false;

                numberBtns[n].interactable = allowed && !removed;
                numberBtns[n].gameObject.SetActive(!removed);

                bool isSelected = selected.Contains(n)
                    || (!needsSource && pickerPlak == PlakId.Green && n == sourceNumber);

                // Apply current plak-driven color; SELECTED overrides for visibility.
                var nc = fx.GetNumberColor(n);
                Color fill;
                if (!allowed) fill = DISABLED;
                else if (isSelected) fill = SELECTED;
                else fill = EffectiveColor(nc, n);

                if (numberImages[n] != null) numberImages[n].color = fill;

                // Label: show resolved number so Green-plak replacements are visible.
                if (numberLabels[n] != null)
                {
                    int resolved = fx.ResolveNumber(n);
                    numberLabels[n].text = resolved == n ? n.ToString() : $"{n}\u2192{resolved}";
                    numberLabels[n].color = LabelColorFor(fill);
                }

                // Badge
                if (numberBadges[n] != null)
                {
                    if (badgeText.TryGetValue(n, out var t))
                    {
                        numberBadges[n].text = t;
                        numberBadges[n].color = badgeColor[n];
                    }
                    else
                    {
                        numberBadges[n].text = "";
                    }
                }
            }

            if (pickerConfirm != null)
                pickerConfirm.interactable = (needsSource && sourceNumber < 0)
                    ? false
                    : selected.Count == picksNeeded;
        }

        static Color LabelColorFor(Color fill)
        {
            // White/yellow backgrounds need dark text for readability.
            float luminance = fill.r * 0.299f + fill.g * 0.587f + fill.b * 0.114f;
            return luminance > 0.7f ? new Color(0.05f, 0.05f, 0.05f) : Color.white;
        }

        void OnNumberClicked(int n)
        {
            if (needsSource && sourceNumber < 0)
            {
                sourceNumber = n;
                UpdatePickerPrompt();
                UpdateNumberButtons();
                return;
            }

            if (selected.Contains(n)) selected.Remove(n);
            else if (selected.Count < picksNeeded) selected.Add(n);

            UpdatePickerPrompt();
            UpdateNumberButtons();
        }

        void OnPickerConfirm()
        {
            if (needsSource && sourceNumber < 0) return;
            if (selected.Count != picksNeeded) return;

            int[] payload;
            if (needsSource)
            {
                payload = new int[1 + selected.Count];
                payload[0] = sourceNumber;
                for (int i = 0; i < selected.Count; i++) payload[i + 1] = selected[i];
            }
            else
            {
                payload = selected.ToArray();
            }

            plakShop.Apply(pickerPlak, payload);
            HideOverlay();
        }

        void OnPickerBack()
        {
            // Return to offers — boss chip remains spent, player must still choose.
            selected.Clear();
            sourceNumber = -1;
            ShowOffers();
        }

        void Refresh()
        {
            // Spend button label is refreshed in Update to track boss chip count live.
        }

    }
}
