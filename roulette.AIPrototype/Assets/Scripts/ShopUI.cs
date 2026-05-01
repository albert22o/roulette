using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeadlyRoulette
{
    [Serializable]
    public class ShopSlotUI
    {
        public GameObject root;
        public Text nameText;
        public Text descText;
        public Text costText;
        public Text rarityText;
        public Button buyButton;
    }

    [Serializable]
    public class InventoryEntryUI
    {
        public GameObject root;
        public Text nameText;
        public Button sellButton;
        public Text sellPriceText;
    }

    /// <summary>
    /// Binds the shop scene hierarchy to ShopManager. All references assigned in Inspector.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] ShopManager shop;
        [SerializeField] RouletteGame game;

        [Header("Main Panel")]
        [SerializeField] GameObject shopPanel;
        [SerializeField] Text goldText;

        [Header("Random Pool (5 slots)")]
        [SerializeField] ShopSlotUI[] randomPoolUI;

        [Header("Food (1 slot)")]
        [SerializeField] ShopSlotUI foodUI;

        [Header("Other Items (2 ball-ticket slots)")]
        [SerializeField] ShopSlotUI[] otherUI;

        [Header("Base Money Upgrade (always available)")]
        [SerializeField] GameObject baseMoneyRoot;
        [SerializeField] Text baseMoneyNameText;
        [SerializeField] Text baseMoneyDescText;
        [SerializeField] Text baseMoneyCostText;
        [SerializeField] Button baseMoneyBuyBtn;

        [Header("Inventory - Chips")]
        [SerializeField] InventoryEntryUI[] chipEntries; // pre-created, max ~10

        [Header("Inventory - Balls")]
        [SerializeField] InventoryEntryUI[] ballEntries; // pre-created, max ~6

        [Header("Number Selection Popup")]
        [SerializeField] GameObject numberPopup;
        [SerializeField] Text numberPromptText;
        [SerializeField] Button[] numberButtons;    // 0-36, index = number
        [SerializeField] Button numberConfirmBtn;

        [Header("Group Selection Popup")]
        [SerializeField] GameObject groupPopup;
        [SerializeField] Text groupPromptText;
        [SerializeField] Button[] groupButtons;     // 3 for dozen, 6 for six
        [SerializeField] Button groupConfirmBtn;

        [Header("Ball Ticket Popup")]
        [SerializeField] GameObject ballPopup;
        [SerializeField] Text ballNameText;
        [SerializeField] Text ballDescText;
        [SerializeField] Button acceptBallBtn;
        [SerializeField] Button sellBallBtn;

        [Header("Buttons")]
        [SerializeField] Button rerollBtn;
        [SerializeField] Text rerollCostText;
        [SerializeField] Button closeBtn;

        // --- State for pending operations ---
        ShopSlot pendingChipSlot;
        ChipId pendingChipId;
        int numberChoicesNeeded;
        List<int> selectedNumbers = new List<int>();
        int selectedGroup;

        BallInstance pendingBall;

        static readonly Color SELECTED_COLOR = new Color(1f, 0.92f, 0.25f, 1f);
        static readonly Color NORMAL_COLOR   = Color.white;

        void Awake()
        {
            if (shop == null) shop = GetComponent<ShopManager>();
            if (game == null) game = GetComponent<RouletteGame>();
        }

        void OnEnable()
        {
            if (shop != null) shop.OnShopChanged += Refresh;
            EnsureRerollButton();
            HookButtons();
            Refresh();
        }

        // Lazily build a reroll button above the random pool if the scene doesn't
        // already have one wired. Keeps the scene authoring optional.
        void EnsureRerollButton()
        {
            if (rerollBtn != null && rerollCostText != null) return;
            if (shopPanel == null) return;

            var go = new GameObject("RerollButton",
                typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(shopPanel.transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot     = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20f, -20f);
            rt.sizeDelta = new Vector2(180f, 48f);

            var img = go.GetComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.35f, 1f);

            var labelGo = new GameObject("Label",
                typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            var text = labelGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = "Reroll (2g)";

            rerollBtn = go.GetComponent<Button>();
            rerollCostText = text;
        }

        void OnDisable()
        {
            if (shop != null) shop.OnShopChanged -= Refresh;
        }

        void HookButtons()
        {
            // Random pool buy buttons
            if (randomPoolUI != null)
            {
                for (int i = 0; i < randomPoolUI.Length; i++)
                {
                    int idx = i;
                    Bind(randomPoolUI[i].buyButton, () => OnBuyRandomPool(idx));
                }
            }

            // Food buy button
            if (foodUI != null)
                Bind(foodUI.buyButton, () => OnBuyFood());

            // Other items buy buttons
            if (otherUI != null)
            {
                for (int i = 0; i < otherUI.Length; i++)
                {
                    int idx = i;
                    Bind(otherUI[i].buyButton, () => OnBuyOther(idx));
                }
            }

            // Number popup buttons
            if (numberButtons != null)
            {
                for (int i = 0; i < numberButtons.Length; i++)
                {
                    int num = i;
                    Bind(numberButtons[i], () => OnNumberClicked(num));
                }
            }
            Bind(numberConfirmBtn, OnNumberConfirm);

            // Group popup buttons
            if (groupButtons != null)
            {
                for (int i = 0; i < groupButtons.Length; i++)
                {
                    int g = i + 1;
                    Bind(groupButtons[i], () => OnGroupClicked(g));
                }
            }
            Bind(groupConfirmBtn, OnGroupConfirm);

            // Ball popup
            Bind(acceptBallBtn, OnAcceptBall);
            Bind(sellBallBtn, OnSellBall);

            // Reroll shop
            Bind(rerollBtn, OnRerollClicked);

            // Base money upgrade
            Bind(baseMoneyBuyBtn, OnBuyBaseMoneyUpgrade);

            // Continue (close shop and start next quota)
            Bind(closeBtn, OnContinueClicked);

            // Chip sell buttons
            if (chipEntries != null)
            {
                for (int i = 0; i < chipEntries.Length; i++)
                {
                    int idx = i;
                    Bind(chipEntries[i].sellButton, () => OnSellChip(idx));
                }
            }

            // Ball sell buttons
            if (ballEntries != null)
            {
                for (int i = 0; i < ballEntries.Length; i++)
                {
                    int idx = i;
                    Bind(ballEntries[i].sellButton, () => OnSellBallEntry(idx));
                }
            }
        }

        // ==============================================================
        //  Refresh UI
        // ==============================================================

        void Refresh()
        {
            if (game == null || shop == null) return;

            if (goldText) goldText.text = "Gold: " + game.goldChips;

            // Reroll button
            if (rerollBtn) rerollBtn.interactable = shop.CanReroll();
            if (rerollCostText) rerollCostText.text = "Reroll (" + shop.RerollCost + "g)";

            // Base money upgrade — always visible & available while affordable
            if (baseMoneyRoot) baseMoneyRoot.SetActive(true);
            if (baseMoneyNameText) baseMoneyNameText.text = "Base Money +" + ShopManager.BaseMoneyUpgradeAmount;
            if (baseMoneyDescText) baseMoneyDescText.text = "Permanently increase base bankroll.";
            if (baseMoneyCostText) baseMoneyCostText.text = shop.CurrentBaseMoneyUpgradeCost.ToString();
            if (baseMoneyBuyBtn)  baseMoneyBuyBtn.interactable = shop.CanAfford(shop.CurrentBaseMoneyUpgradeCost);

            var inv = game.inventory;
            var fx = new EffectProcessor(inv);

            // Random pool
            if (randomPoolUI != null)
            {
                for (int i = 0; i < randomPoolUI.Length && i < shop.randomPool.Length; i++)
                    RefreshSlotUI(randomPoolUI[i], shop.randomPool[i]);
            }

            // Food
            if (foodUI != null)
                RefreshSlotUI(foodUI, shop.foodSlot);

            // Other
            if (otherUI != null)
            {
                for (int i = 0; i < otherUI.Length && i < shop.otherSlots.Length; i++)
                    RefreshSlotUI(otherUI[i], shop.otherSlots[i]);
            }

            // Inventory: chips
            if (chipEntries != null)
            {
                for (int i = 0; i < chipEntries.Length; i++)
                {
                    bool hasChip = i < inv.equippedChips.Count && !inv.equippedChips[i].Destroyed;
                    if (chipEntries[i].root) chipEntries[i].root.SetActive(hasChip);
                    if (hasChip)
                    {
                        var chip = inv.equippedChips[i];
                        if (chipEntries[i].nameText)
                            chipEntries[i].nameText.text = ItemRegistry.Chips[chip.Id].Name;
                        if (chipEntries[i].sellPriceText)
                            chipEntries[i].sellPriceText.text = shop.GetChipSellPrice(chip.Id).ToString();
                    }
                }
            }

            // Inventory: balls
            if (ballEntries != null)
            {
                for (int i = 0; i < ballEntries.Length; i++)
                {
                    bool hasBall = i < inv.ownedBalls.Count && !inv.ownedBalls[i].Destroyed;
                    if (ballEntries[i].root) ballEntries[i].root.SetActive(hasBall);
                    if (hasBall)
                    {
                        var ball = inv.ownedBalls[i];
                        if (ballEntries[i].nameText)
                            ballEntries[i].nameText.text = ItemRegistry.Balls[ball.Id].Name;
                        bool canSell = shop.CanSellBall(inv);
                        if (ballEntries[i].sellButton)
                            ballEntries[i].sellButton.interactable = canSell;
                        if (ballEntries[i].sellPriceText)
                            ballEntries[i].sellPriceText.text = "3";
                    }
                }
            }
        }

        void RefreshSlotUI(ShopSlotUI ui, ShopSlot slot)
        {
            if (ui == null || ui.root == null) return;

            bool available = slot != null && !slot.empty;
            ui.root.SetActive(true);

            if (ui.nameText) ui.nameText.text = available ? slot.DisplayName : "-- SOLD --";
            if (ui.descText) ui.descText.text = available ? slot.Description : "";
            if (ui.costText) ui.costText.text = available ? slot.cost.ToString() : "";
            if (ui.rarityText) ui.rarityText.text = available ? slot.RarityLabel : "";
            if (ui.buyButton)
            {
                ui.buyButton.gameObject.SetActive(available);
                ui.buyButton.interactable = available && shop.CanAfford(slot.cost);
            }
        }

        // ==============================================================
        //  Buy handlers
        // ==============================================================

        void OnBuyRandomPool(int index)
        {
            if (index < 0 || index >= shop.randomPool.Length) return;
            var slot = shop.randomPool[index];
            if (slot == null || slot.empty) return;

            if (slot.kind == ShopItemKind.Chip)
            {
                var inv = game.inventory;
                var fx = new EffectProcessor(inv);
                if (!shop.CanBuyChip(inv, fx)) return;
                StartChipPurchase(slot);
            }
            else if (slot.kind == ShopItemKind.Card)
            {
                shop.BuyCard(slot, game.inventory);
            }
        }

        void OnBuyFood()
        {
            shop.BuyFood(game.inventory);
        }

        void OnBuyOther(int index)
        {
            if (index < 0 || index >= shop.otherSlots.Length) return;
            var slot = shop.otherSlots[index];
            if (slot == null || slot.empty) return;

            switch (slot.kind)
            {
                case ShopItemKind.BallTicket:
                    var inv = game.inventory;
                    var fx = new EffectProcessor(inv);
                    if (!shop.CanBuyBall(inv, fx)) return;
                    var ball = shop.BuyBallTicket(slot);
                    if (ball != null) ShowBallPopup(ball);
                    break;
            }
        }

        // ==============================================================
        //  Chip purchase flow
        // ==============================================================

        void StartChipPurchase(ShopSlot slot)
        {
            pendingChipSlot = slot;
            pendingChipId = slot.chipId;

            var mode = ShopManager.GetSelectionMode(pendingChipId);

            switch (mode)
            {
                case ChipSelectionMode.Numbers:
                    numberChoicesNeeded = ShopManager.GetNumberChoiceCount(pendingChipId);
                    selectedNumbers.Clear();
                    ShowNumberPopup();
                    break;

                case ChipSelectionMode.Dozen:
                    selectedGroup = 0;
                    ShowGroupPopup(3, "Select a dozen (1-12, 13-24, 25-36):");
                    break;

                case ChipSelectionMode.None:
                    shop.BuyChip(pendingChipSlot, game.inventory, null, 0);
                    break;
            }
        }

        // --- Number popup ---

        void ShowNumberPopup()
        {
            if (numberPopup) numberPopup.SetActive(true);
            bool allowZero = ShopManager.CanSelectZero(pendingChipId);

            if (numberPromptText)
                numberPromptText.text = "Select " + numberChoicesNeeded + " number(s):";

            if (numberButtons != null)
            {
                for (int i = 0; i < numberButtons.Length; i++)
                {
                    if (numberButtons[i] == null) continue;
                    numberButtons[i].interactable = (i > 0 || allowZero);
                    SetButtonColor(numberButtons[i], NORMAL_COLOR);
                }
            }

            if (numberConfirmBtn) numberConfirmBtn.interactable = false;
        }

        void OnNumberClicked(int num)
        {
            if (selectedNumbers.Contains(num))
            {
                selectedNumbers.Remove(num);
                SetButtonColor(numberButtons[num], NORMAL_COLOR);
            }
            else if (selectedNumbers.Count < numberChoicesNeeded)
            {
                selectedNumbers.Add(num);
                SetButtonColor(numberButtons[num], SELECTED_COLOR);
            }

            if (numberConfirmBtn)
                numberConfirmBtn.interactable = selectedNumbers.Count == numberChoicesNeeded;
        }

        void OnNumberConfirm()
        {
            if (selectedNumbers.Count != numberChoicesNeeded) return;
            if (numberPopup) numberPopup.SetActive(false);

            shop.BuyChip(pendingChipSlot, game.inventory, selectedNumbers.ToArray(), 0);
            pendingChipSlot = null;
        }

        // --- Group popup ---

        void ShowGroupPopup(int count, string prompt)
        {
            if (groupPopup) groupPopup.SetActive(true);
            if (groupPromptText) groupPromptText.text = prompt;

            if (groupButtons != null)
            {
                for (int i = 0; i < groupButtons.Length; i++)
                {
                    if (groupButtons[i] == null) continue;
                    groupButtons[i].gameObject.SetActive(i < count);
                    SetButtonColor(groupButtons[i], NORMAL_COLOR);
                }
            }

            selectedGroup = 0;
            if (groupConfirmBtn) groupConfirmBtn.interactable = false;
        }

        void OnGroupClicked(int group)
        {
            selectedGroup = group;

            if (groupButtons != null)
            {
                for (int i = 0; i < groupButtons.Length; i++)
                {
                    if (groupButtons[i] == null) continue;
                    SetButtonColor(groupButtons[i], (i + 1 == group) ? SELECTED_COLOR : NORMAL_COLOR);
                }
            }

            if (groupConfirmBtn) groupConfirmBtn.interactable = true;
        }

        void OnGroupConfirm()
        {
            if (selectedGroup <= 0) return;
            if (groupPopup) groupPopup.SetActive(false);

            shop.BuyChip(pendingChipSlot, game.inventory, null, selectedGroup);
            pendingChipSlot = null;
        }

        // ==============================================================
        //  Ball ticket popup
        // ==============================================================

        void ShowBallPopup(BallInstance ball)
        {
            pendingBall = ball;
            if (ballPopup) ballPopup.SetActive(true);

            var def = ItemRegistry.Balls[ball.Id];
            if (ballNameText) ballNameText.text = def.Name;
            if (ballDescText) ballDescText.text = def.Description;
        }

        void OnAcceptBall()
        {
            if (pendingBall == null) return;
            if (ballPopup) ballPopup.SetActive(false);
            shop.AcceptBall(game.inventory, pendingBall);
            pendingBall = null;
        }

        void OnSellBall()
        {
            if (ballPopup) ballPopup.SetActive(false);
            shop.SellRolledBall();
            pendingBall = null;
        }

        // ==============================================================
        //  Sell handlers
        // ==============================================================

        void OnSellChip(int index)
        {
            var inv = game.inventory;
            if (index < 0 || index >= inv.equippedChips.Count) return;
            shop.SellChip(inv, inv.equippedChips[index]);
        }

        void OnSellBallEntry(int index)
        {
            var inv = game.inventory;
            if (index < 0 || index >= inv.ownedBalls.Count) return;
            shop.SellBall(inv, inv.ownedBalls[index]);
        }

        // ==============================================================
        //  Public
        // ==============================================================

        public bool IsOpen => shopPanel != null && shopPanel.activeSelf;

        public void OpenShop()
        {
            shop.GenerateShop();
            if (shopPanel) shopPanel.SetActive(true);
            Refresh();
        }

        void OnRerollClicked()
        {
            shop.RerollShop();
        }

        void OnBuyBaseMoneyUpgrade()
        {
            shop.BuyBaseMoneyUpgrade(game.inventory);
        }

        void OnContinueClicked()
        {
            if (shopPanel) shopPanel.SetActive(false);
            if (game != null && game.Phase == GamePhase.Shopping)
                game.ContinueFromShop();
        }

        // ==============================================================
        //  Helpers
        // ==============================================================

        static void Bind(Button b, UnityEngine.Events.UnityAction a)
        {
            if (b == null) return;
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(a);
        }

        static void SetButtonColor(Button b, Color c)
        {
            if (b == null) return;
            var colors = b.colors;
            colors.normalColor = c;
            b.colors = colors;
        }
    }
}
