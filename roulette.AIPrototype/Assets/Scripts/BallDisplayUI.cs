using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DeadlyRoulette
{
    /// <summary>
    /// Shows owned balls (bottom-left) and spin results with ball selection.
    /// </summary>
    public class BallDisplayUI : MonoBehaviour
    {
        [SerializeField] RouletteGame game;

        [Header("Owned Balls (bottom-left panel)")]
        [SerializeField] GameObject[] ballSlots;        // pre-created, max ~6

        [Header("Spin Results (shown during ResultPending)")]
        [SerializeField] GameObject spinResultsPanel;
        [SerializeField] GameObject[] resultSlots;      // pre-created, max ~6
        // Each result slot should contain: BallNameText, NumberText, SelectButton

        [Header("Tooltip")]
        [SerializeField] GameObject tooltip;
        [SerializeField] Text tooltipName;
        [SerializeField] Text tooltipDesc;

        static readonly Color SELECTED   = new Color(1f,   0.92f, 0.25f, 1f);   // yellow: player-chosen
        static readonly Color CO_SCORING = new Color(0.35f, 0.95f, 0.45f, 1f);  // green: also scoring (e.g. Stone)
        static readonly Color UNSELECTED = new Color(0.35f, 0.35f, 0.4f,  1f);
        static readonly Color NONSCORE   = new Color(0.25f, 0.25f, 0.25f, 0.6f);

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
            RefreshOwnedBalls();
            RefreshSpinResults();
            if (tooltip) tooltip.SetActive(false);
        }

        void RefreshOwnedBalls()
        {
            var inv = game.inventory;
            if (ballSlots == null) return;

            for (int i = 0; i < ballSlots.Length; i++)
            {
                if (ballSlots[i] == null) continue;
                bool active = i < inv.ownedBalls.Count && !inv.ownedBalls[i].Destroyed;
                ballSlots[i].SetActive(active);

                if (active)
                {
                    var ball = inv.ownedBalls[i];
                    var def = ItemRegistry.Balls[ball.Id];

                    var label = ballSlots[i].GetComponentInChildren<Text>();
                    if (label != null) label.text = def.Name;

                    var handler = ballSlots[i].GetComponent<BallSlotHandler>();
                    if (handler == null) handler = ballSlots[i].AddComponent<BallSlotHandler>();
                    handler.Setup(this, ball);
                }
            }
        }

        void RefreshSpinResults()
        {
            bool showResults = game.Phase == GamePhase.ResultPending && game.ballResults.Count > 0;
            if (spinResultsPanel) spinResultsPanel.SetActive(showResults);
            if (!showResults || resultSlots == null) return;

            var fx = new EffectProcessor(game.inventory);

            for (int i = 0; i < resultSlots.Length; i++)
            {
                if (resultSlots[i] == null) continue;
                bool active = i < game.ballResults.Count;
                resultSlots[i].SetActive(active);

                if (!active) continue;

                var br = game.ballResults[i];
                var def = ItemRegistry.Balls[br.Ball.Id];

                // Find child texts
                var texts = resultSlots[i].GetComponentsInChildren<Text>();
                // Convention: first Text = ball name, second = number
                if (texts.Length >= 1) texts[0].text = def.Name;
                if (texts.Length >= 2)
                {
                    int shown = fx.ResolveNumber(br.RolledNumber);
                    texts[1].text = shown + " (" + EffectiveColorName(fx, shown) + ")";
                }

                // Highlight:
                //  - NONSCORE   : gray/dim (not contributing)
                //  - SELECTED   : yellow (primary chosen ball)
                //  - CO_SCORING : green  (scores alongside primary — e.g. Stone, or extra picks via Checkmark)
                var img = resultSlots[i].GetComponent<Image>();
                if (img != null)
                {
                    if (!br.Scores)
                        img.color = NONSCORE;
                    else if (i == game.selectedBallIndex)
                        img.color = SELECTED;
                    else
                        img.color = CO_SCORING;
                }

                // Select button — any ball can be chosen; SelectBall re-derives scoring.
                var btn = resultSlots[i].GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                    int idx = i;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => game.SelectBall(idx));
                }

                // Hover handler
                var handler = resultSlots[i].GetComponent<BallResultHandler>();
                if (handler == null) handler = resultSlots[i].AddComponent<BallResultHandler>();
                handler.Setup(this, br.Ball);
            }
        }

        public void ShowTooltip(BallInstance ball)
        {
            if (tooltip == null || ball == null) return;
            var def = ItemRegistry.Balls[ball.Id];
            if (tooltipName) tooltipName.text = def.Name;
            if (tooltipDesc) tooltipDesc.text = def.Description;
            tooltip.SetActive(true);
        }

        public void HideTooltip()
        {
            if (tooltip) tooltip.SetActive(false);
        }

        public static string EffectiveColorName(EffectProcessor fx, int n)
        {
            var nc = fx != null ? fx.GetNumberColor(n) : NumberColor.Default;
            switch (nc)
            {
                case NumberColor.Red:    return "red";
                case NumberColor.Black:  return "black";
                case NumberColor.Gray:   return "gray";
                case NumberColor.Purple: return "purple";
                case NumberColor.Blue:   return "blue";
                case NumberColor.White:  return "white";
                case NumberColor.Yellow: return "yellow";
                case NumberColor.Orange: return "orange";
            }
            if (n == 0) return "green";
            return RouletteData.ColorOf(n) == BetColor.Red ? "red" : "black";
        }
    }

    /// <summary>Hover handler for owned ball slots.</summary>
    public class BallSlotHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        BallDisplayUI display;
        BallInstance ball;

        public void Setup(BallDisplayUI d, BallInstance b) { display = d; ball = b; }

        public void OnPointerEnter(PointerEventData e)
        {
            if (display != null) display.ShowTooltip(ball);
        }
        public void OnPointerExit(PointerEventData e)
        {
            if (display != null) display.HideTooltip();
        }
    }

    /// <summary>Hover handler for spin result slots.</summary>
    public class BallResultHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        BallDisplayUI display;
        BallInstance ball;

        public void Setup(BallDisplayUI d, BallInstance b) { display = d; ball = b; }

        public void OnPointerEnter(PointerEventData e)
        {
            if (display != null) display.ShowTooltip(ball);
        }
        public void OnPointerExit(PointerEventData e)
        {
            if (display != null) display.HideTooltip();
        }
    }
}
