using System;
using UnityEngine;
using UnityEngine.UI;

namespace DeadlyRoulette
{
    [Serializable]
    public class PostBossSlotUI
    {
        public GameObject root;
        public Text nameText;
        public Text descText;
        public Text kindText;
        public Button claimButton;
        public Text claimedLabel;
    }

    /// <summary>
    /// Binds the post-boss upgrade scene hierarchy to PostBossManager.
    /// All references assigned in Inspector by PostBossUIBuilder.
    /// </summary>
    public class PostBossUI : MonoBehaviour
    {
        [SerializeField] PostBossManager mgr;
        [SerializeField] RouletteGame game;

        [Header("Main Panel")]
        [SerializeField] GameObject panel;
        [SerializeField] Text titleText;
        [SerializeField] Text goldText;

        [Header("Slots (5)")]
        [SerializeField] PostBossSlotUI[] slotUI;

        [Header("Ball Popup")]
        [SerializeField] GameObject ballPopup;
        [SerializeField] Text ballNameText;
        [SerializeField] Text ballDescText;
        [SerializeField] Button acceptBallBtn;
        [SerializeField] Button sellBallBtn;

        [Header("Continue")]
        [SerializeField] Button continueBtn;

        void Awake()
        {
            if (mgr == null) mgr = GetComponent<PostBossManager>();
            if (game == null) game = GetComponent<RouletteGame>();
        }

        void OnEnable()
        {
            if (mgr != null) mgr.OnChanged += Refresh;
            HookButtons();
            Refresh();
        }

        void OnDisable()
        {
            if (mgr != null) mgr.OnChanged -= Refresh;
        }

        void HookButtons()
        {
            if (slotUI != null)
            {
                for (int i = 0; i < slotUI.Length; i++)
                {
                    int idx = i;
                    Bind(slotUI[i].claimButton, () => OnClaim(idx));
                }
            }
            Bind(acceptBallBtn, OnAcceptBall);
            Bind(sellBallBtn, OnSellBall);
            Bind(continueBtn, OnContinue);
        }

        public bool IsOpen => panel != null && panel.activeSelf;

        public void Open()
        {
            mgr.Generate();
            if (panel) panel.SetActive(true);
            Refresh();
        }

        void Refresh()
        {
            if (game == null || mgr == null) return;
            if (goldText) goldText.text = "Gold: " + game.goldChips;

            if (slotUI != null)
            {
                for (int i = 0; i < slotUI.Length && i < mgr.slots.Length; i++)
                    RefreshSlot(slotUI[i], mgr.slots[i]);
            }

            // Show ball popup when a ball is pending.
            if (ballPopup)
            {
                bool show = mgr.pendingBall != null;
                ballPopup.SetActive(show);
                if (show)
                {
                    var def = ItemRegistry.Balls[mgr.pendingBall.Id];
                    if (ballNameText) ballNameText.text = def.Name;
                    if (ballDescText) ballDescText.text = def.Description;
                }
            }

            // One-choice-only: Continue enabled as soon as any slot is claimed
            // and no ball decision is pending.
            bool anyClaimed = false;
            foreach (var s in mgr.slots)
                if (s != null && s.claimed) { anyClaimed = true; break; }
            if (continueBtn) continueBtn.interactable = anyClaimed && mgr.pendingBall == null;
        }

        void RefreshSlot(PostBossSlotUI ui, PostBossSlot slot)
        {
            if (ui == null || ui.root == null || slot == null) return;
            ui.root.SetActive(true);

            if (ui.nameText) ui.nameText.text = slot.DisplayName;
            if (ui.descText) ui.descText.text = slot.Description;
            if (ui.kindText) ui.kindText.text = slot.kind.ToString();
            if (ui.claimButton)
            {
                ui.claimButton.gameObject.SetActive(!slot.claimed);
                ui.claimButton.interactable = !slot.claimed && !mgr.AnyClaimed;
            }
            if (ui.claimedLabel)
                ui.claimedLabel.gameObject.SetActive(slot.claimed);
        }

        void OnClaim(int index) => mgr.Claim(index);
        void OnAcceptBall()     => mgr.AcceptPendingBall();
        void OnSellBall()       => mgr.SellPendingBall();

        void OnContinue()
        {
            if (panel) panel.SetActive(false);
            if (game != null && game.Phase == GamePhase.PostBossUpgrade)
                game.ContinueFromPostBoss();
        }

        static void Bind(Button b, UnityEngine.Events.UnityAction a)
        {
            if (b == null) return;
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(a);
        }
    }
}
