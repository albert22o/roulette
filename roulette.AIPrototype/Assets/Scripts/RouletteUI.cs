using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DeadlyRoulette
{
    [Serializable]
    public class NumberCellRef
    {
        public int number;
        public Button button;
        public GameObject chipBg;        // shown when stake > 0
        public Text chipLabel;           // amount text
        public Image highlight;          // win highlight overlay
    }

    /// <summary>
    /// Binds a pre-built scene hierarchy to the RouletteGame state machine.
    /// All references are assigned in the Inspector — this script does NOT build UI.
    /// </summary>
    public class RouletteUI : MonoBehaviour
    {
        [SerializeField] RouletteGame game;

        [Header("HUD")]
        [SerializeField] Text hudMoney;
        [SerializeField] Text hudQuota;
        [SerializeField] Text hudBets;
        [SerializeField] Text hudRerolls;
        [SerializeField] Text hudChips;
        [SerializeField] Text hudPhase;
        [SerializeField] Text hudResult;

        [Header("Number cells (one entry per number, including 0)")]
        [SerializeField] NumberCellRef[] numberCells;

        // Runtime overlay labels for chip effects (Seven "+4", MoreLess "±2", etc.)
        readonly Dictionary<int, Text> cellOverlays = new Dictionary<int, Text>();

        [Header("Category highlights")]
        [SerializeField] Image redHi;
        [SerializeField] Image blackHi;
        [SerializeField] Image[] dozenHi;   // length 3

        [Header("Category stake labels")]
        [SerializeField] Text redStakeText;
        [SerializeField] Text blackStakeText;
        [SerializeField] Text[] dozenStakeText;  // length 3

        [Header("Category multiplier labels")]
        [SerializeField] Text redMultLabel;
        [SerializeField] Text blackMultLabel;
        [SerializeField] Text[] dozenMultLabel;  // length 3
        [SerializeField] Text numbersMultLabel;
        [SerializeField] Text zeroMultLabel;

        [Header("Multiplier Breakdown Panel (left side)")]
        [SerializeField] GameObject breakdownPanel;
        [SerializeField] Text breakdownText;

        [Header("Buttons")]
        [SerializeField] Button confirmBetBtn;
        [SerializeField] Button clearBetBtn;
        [SerializeField] Button confirmResultBtn;
        [SerializeField] Button rerollBtn;
        [SerializeField] Button continueBtn;
        [SerializeField] Button endQuotaBtn;
        [SerializeField] Button restartBtn;
        [SerializeField] Button shopBtn;
        [SerializeField] ShopUI shopUI;
        [SerializeField] PostBossUI postBossUI;

        void Awake()
        {
            if (game == null) game = GetComponent<RouletteGame>();
            HookButtons();
        }

        void OnEnable()
        {
            if (game != null) game.OnStateChanged += Refresh;
        }
        void OnDisable()
        {
            if (game != null) game.OnStateChanged -= Refresh;
        }

        void Start() { Refresh(); }

        void HookButtons()
        {
            // Number cells: click opens the bet popup for that number.
            if (numberCells != null)
            {
                foreach (var cell in numberCells)
                {
                    if (cell == null || cell.button == null) continue;
                    int n = cell.number;
                    cell.button.onClick.RemoveAllListeners();
                    cell.button.onClick.AddListener(() => ShowBetPopup(n));

                    // Remove any legacy right-click handler (old betting UI used +/- 25).
                    var rc = cell.button.GetComponent<RightClickHandler>();
                    if (rc != null) Destroy(rc);
                }
            }

            Bind(confirmBetBtn,    () => game.ConfirmBetAndSpin());
            Bind(clearBetBtn,      () => game.ClearBet());
            Bind(confirmResultBtn, () => game.ConfirmResult());
            Bind(rerollBtn,        () => game.Reroll());
            Bind(continueBtn,      () => game.ContinueAfterBet());
            Bind(endQuotaBtn,      () => game.EndQuota());
            Bind(restartBtn,       () => game.RestartFromGameOver());
            Bind(shopBtn,          () => { if (shopUI != null) shopUI.OpenShop(); });
        }

        static void Bind(Button b, UnityEngine.Events.UnityAction a)
        {
            if (b == null) return;
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(a);
        }

        static readonly Color HI      = new Color(1f, 0.92f, 0.25f, 0.35f);  // secondary winning zones
        static readonly Color HI_BEST = new Color(1f, 0.92f, 0.25f, 0.70f);  // best category highlight
        static readonly Color CLEAR   = new Color(1f, 1f, 1f, 0f);

        // Recolor overlay tints for card-driven NumberColor changes.
        static readonly Color TINT_RED    = new Color(1f,   0.25f, 0.25f, 0.55f);
        static readonly Color TINT_BLACK  = new Color(0.1f, 0.1f,  0.1f,  0.70f);
        static readonly Color TINT_GRAY   = new Color(0.5f, 0.5f,  0.5f,  0.65f);
        static readonly Color TINT_PURPLE = new Color(0.6f, 0.2f,  0.75f, 0.65f);
        static readonly Color TINT_BLUE   = new Color(0.3f, 0.55f, 1f,    0.60f);
        static readonly Color TINT_WHITE  = new Color(1f,   1f,    1f,    0.80f);
        static readonly Color TINT_YELLOW = new Color(1f,   0.95f, 0.25f, 0.60f);
        static readonly Color TINT_ORANGE = new Color(1f,   0.6f,  0.15f, 0.60f);
        // MoreLess ± range overlay
        static readonly Color RANGE_TINT  = new Color(0.4f, 0.85f, 1f,    0.28f);
        // Spikes overlay — tiles that have been replaced with the chosen number
        static readonly Color SPIKE_TINT  = new Color(1f,   0.45f, 0.1f,  0.55f);
        // Seven chip — subtle gold tint on target numbers
        static readonly Color SEVEN_TINT  = new Color(1f,   0.85f, 0.3f,  0.35f);

        static Color NumberColorTint(NumberColor nc)
        {
            switch (nc)
            {
                case NumberColor.Red:    return TINT_RED;
                case NumberColor.Black:  return TINT_BLACK;
                case NumberColor.Gray:   return TINT_GRAY;
                case NumberColor.Purple: return TINT_PURPLE;
                case NumberColor.Blue:   return TINT_BLUE;
                case NumberColor.White:  return TINT_WHITE;
                case NumberColor.Yellow: return TINT_YELLOW;
                case NumberColor.Orange: return TINT_ORANGE;
                default: return CLEAR;
            }
        }

        Text GetOverlayText(NumberCellRef cell)
        {
            if (cellOverlays.TryGetValue(cell.number, out var t) && t != null)
                return t;
            var go = new GameObject("EffectLabel", typeof(RectTransform), typeof(Text), typeof(Outline));
            go.transform.SetParent(cell.button.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0, 2);
            rt.sizeDelta = new Vector2(80, 20);
            t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 14;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var outline = go.GetComponent<Outline>();
            outline.effectColor = new Color(0, 0, 0, 0.8f);
            outline.effectDistance = new Vector2(1, -1);
            cellOverlays[cell.number] = t;
            return t;
        }

        void Refresh()
        {
            if (game == null) return;

            if (hudMoney)
            {
                bool quotaMet = game.quotaProgress >= game.CurrentQuota;
                string bossTag = game.CurrentIsBoss ? "[BOSS] " : "";
                hudMoney.text  = $"{bossTag}Won: {game.quotaProgress} / {game.CurrentQuota}";
                hudMoney.color = quotaMet ? new Color(0.5f, 1f, 0.5f)
                                : game.CurrentIsBoss ? new Color(1f, 0.6f, 0.4f)
                                : Color.white;
            }
            if (hudQuota)
            {
                int penalty = game.goldPenalty;
                hudQuota.text = penalty > 0 ? $"Penalty: -{penalty}g" : "";
            }
            if (hudBets)    hudBets.text    = $"Bets: {game.betsRemaining}/{RouletteGame.BetsPerQuota}";
            if (hudRerolls) hudRerolls.text = $"Rerolls: {game.rerollsRemaining}/{RouletteGame.RerollsPerQuota}";
            if (hudChips)   hudChips.text   = $"Gold: {game.goldChips}   Boss Chips: {game.inventory.bossChips}";
            if (hudPhase)   hudPhase.text   = $"Phase: {game.Phase}   Money: ${game.baseMoney - game.TotalStake}   Stake: ${game.TotalStake}";

            if (numberCells != null)
            {
                var fxRemoved = new EffectProcessor(game.inventory);
                foreach (var cell in numberCells)
                {
                    if (cell == null) continue;

                    // Boom / BigBang: hide the whole cell. The wheel skips removed numbers
                    // too, so the UI should not show them either.
                    bool removed = fxRemoved.IsNumberRemoved(cell.number);
                    if (cell.button != null) cell.button.gameObject.SetActive(!removed);
                    if (removed) continue;

                    int amt = game.currentBet.TryGetValue(cell.number, out var v) ? v : 0;
                    if (cell.chipBg)    cell.chipBg.SetActive(amt > 0);
                    if (cell.chipLabel) cell.chipLabel.text = amt > 0 ? amt.ToString() : "";
                    if (cell.highlight) cell.highlight.color = CLEAR;
                    if (cellOverlays.TryGetValue(cell.number, out var overlay) && overlay != null)
                        overlay.text = "";
                }
            }

            // Per-category stake totals (numbers contribute to their color/dozen; 0 contributes to none)
            int redTotal = 0, blackTotal = 0;
            int[] dozenTotals = new int[3];
            foreach (var kv in game.currentBet)
            {
                int n = kv.Key, amt = kv.Value;
                if (n == 0) continue;
                if (RouletteData.ColorOf(n) == BetColor.Red) redTotal += amt; else blackTotal += amt;
                dozenTotals[RouletteData.DozenOf(n) - 1] += amt;
            }
            SetStake(redStakeText,   redTotal);
            SetStake(blackStakeText, blackTotal);
            if (dozenStakeText != null)
                for (int i = 0; i < dozenStakeText.Length && i < 3; i++) SetStake(dozenStakeText[i], dozenTotals[i]);

            // Multiplier labels (always visible) — include permanent card bonuses.
            int colorMult  = RouletteData.MultiplierFor(BetCategory.Color)  + game.inventory.bonusColorMult;
            int dozenMult  = RouletteData.MultiplierFor(BetCategory.Dozen)  + game.inventory.bonusDozenMult;
            int numberMult = RouletteData.MultiplierFor(BetCategory.Number) + game.inventory.bonusNumberMult;
            int zeroMult   = RouletteData.MultiplierFor(BetCategory.Zero)   + game.inventory.bonusZeroMult;

            SetMultLabel(redMultLabel,   "x" + colorMult);
            SetMultLabel(blackMultLabel, "x" + colorMult);
            if (dozenMultLabel != null)
            {
                // Per-dozen bonus from BlackDozen chips targeting specific dozens
                int[] dozenBonus = new int[3];
                foreach (var c in game.inventory.equippedChips)
                    if (!c.Destroyed && c.Id == ChipId.BlackDozen && c.TargetGroup >= 1 && c.TargetGroup <= 3)
                        dozenBonus[c.TargetGroup - 1] += 2;
                for (int i = 0; i < dozenMultLabel.Length && i < 3; i++)
                    SetMultLabel(dozenMultLabel[i], "x" + (dozenMult + dozenBonus[i]));
            }
            SetMultLabel(numbersMultLabel, "x" + numberMult);
            SetMultLabel(zeroMultLabel,    "x" + zeroMult);

            if (redHi)   redHi.color   = CLEAR;
            if (blackHi) blackHi.color = CLEAR;
            if (dozenHi != null) for (int i = 0; i < dozenHi.Length; i++) if (dozenHi[i]) dozenHi[i].color = CLEAR;

            bool betting    = game.Phase == GamePhase.Betting;
            bool resPending = game.Phase == GamePhase.ResultPending;
            bool postBet    = game.Phase == GamePhase.PostBet;
            bool postBoss   = game.Phase == GamePhase.PostBossUpgrade;
            bool shopping   = game.Phase == GamePhase.Shopping;
            bool gameOver   = game.Phase == GamePhase.GameOver;

            // Number-cell overlay: show card-driven recolors and MoreLess ± range.
            // Only during Betting / ResultPending — PostBet needs the highlight Image
            // for the winning-zone display.
            if ((betting || resPending) && numberCells != null)
            {
                var fx = new EffectProcessor(game.inventory);
                foreach (var cell in numberCells)
                {
                    if (cell == null || cell.highlight == null) continue;
                    if (fx.IsNumberRemoved(cell.number)) continue;

                    var nc = fx.GetNumberColor(cell.number);
                    if (nc != NumberColor.Default)
                    {
                        cell.highlight.color = NumberColorTint(nc);
                        continue;
                    }

                    // Replacement chips: if this cell's number resolves to a different number
                    // (Spikes), tint it.
                    int resolved = fx.ResolveNumber(cell.number);
                    if (resolved != cell.number)
                    {
                        cell.highlight.color = SPIKE_TINT;
                        continue;
                    }
                }

                // Update cell button labels to show resolved numbers so players
                // see the actual number that will score on a replaced position.
                foreach (var cell in numberCells)
                {
                    if (cell == null || cell.button == null) continue;
                    var label = cell.button.GetComponentInChildren<Text>();
                    if (label == null) continue;
                    int resolved = fx.ResolveNumber(cell.number);
                    label.text = resolved.ToString();
                }

                // Chip effect labels (Seven "+4", MoreLess "±2", etc.)
                foreach (var c in game.inventory.equippedChips)
                {
                    if (c.Destroyed || c.TargetNumbers == null) continue;

                    if (c.Id == ChipId.Seven)
                    {
                        foreach (int tn in c.TargetNumbers)
                        {
                            var cell = FindCell(tn);
                            if (cell == null) continue;
                            var ov = GetOverlayText(cell);
                            ov.text = "+4";
                            ov.color = new Color(1f, 0.9f, 0.3f);
                            if (cell.highlight != null && cell.highlight.color.a < 0.01f)
                                cell.highlight.color = SEVEN_TINT;
                        }
                    }
                    else if (c.Id == ChipId.Six)
                    {
                        foreach (int tn in c.TargetNumbers)
                        {
                            var cell = FindCell(tn);
                            if (cell == null) continue;
                            var ov = GetOverlayText(cell);
                            ov.text = "+3";
                            ov.color = new Color(0.6f, 1f, 0.4f);
                            if (cell.highlight != null && cell.highlight.color.a < 0.01f)
                                cell.highlight.color = SEVEN_TINT;
                        }
                    }
                }

                // Plak badges on the main board (PlusMinus, Plus).
                foreach (var p in game.inventory.plaks)
                {
                    if (p.TargetNumbers == null || p.TargetNumbers.Length == 0) continue;
                    if (p.Id == PlakId.PlusMinus)
                    {
                        var cell = FindCell(p.TargetNumbers[0]);
                        if (cell == null) continue;
                        var ov = GetOverlayText(cell);
                        ov.text = "\u00B12";
                        ov.color = new Color(0.4f, 0.9f, 1f);
                    }
                    else if (p.Id == PlakId.Plus)
                    {
                        var cell = FindCell(p.TargetNumbers[0]);
                        if (cell == null) continue;
                        var ov = GetOverlayText(cell);
                        ov.text = "x3";
                        ov.color = new Color(1f, 0.85f, 0.3f);
                    }
                }
            }

            SetInteractable(confirmBetBtn,    betting && game.TotalStake > 0);
            SetInteractable(clearBetBtn,      betting && game.TotalStake > 0);
            SetInteractable(confirmResultBtn, resPending);
            SetInteractable(rerollBtn,        resPending && game.rerollsRemaining > 0);
            SetInteractable(continueBtn,      postBet && game.betsRemaining > 0);
            SetInteractable(endQuotaBtn,      postBet && game.CanEndQuota);
            SetInteractable(restartBtn,       gameOver);
            SetInteractable(shopBtn,          false);

            // Auto-open post-boss screen first, then shop.
            if (postBoss && postBossUI != null && !postBossUI.IsOpen)
                postBossUI.Open();
            if (shopping && shopUI != null && !shopUI.IsOpen)
                shopUI.OpenShop();

            if (hudResult == null) return;
            var fxResolve = new EffectProcessor(game.inventory);
            int lastShown = fxResolve.ResolveNumber(game.lastRolledNumber);
            if (resPending)
            {
                // Multi-ball results summary
                if (game.ballResults.Count > 1)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append("Results: ");
                    for (int i = 0; i < game.ballResults.Count; i++)
                    {
                        var br = game.ballResults[i];
                        var def = ItemRegistry.Balls[br.Ball.Id];
                        // Marker: >> primary chosen, + co-scoring (e.g. Stone), [X] non-scoring
                        string marker;
                        if (i == game.selectedBallIndex) marker = ">> ";
                        else if (br.Scores)               marker = "+  ";
                        else                              marker = "[X] ";
                        int shown = fxResolve.ResolveNumber(br.RolledNumber);
                        sb.Append($"{marker}{def.Name}:{shown} ({BallDisplayUI.EffectiveColorName(fxResolve, shown)})");
                        if (i < game.ballResults.Count - 1) sb.Append("  |  ");
                    }
                    sb.Append("\nAll scoring balls (>> and +) contribute to payout. Select a ball, then Confirm or Reroll.");
                    hudResult.text = sb.ToString();
                    hudResult.color = Color.white;
                }
                else
                {
                    string colName = BallDisplayUI.EffectiveColorName(fxResolve, lastShown);
                    hudResult.text = $"Rolled: {lastShown}  ({colName})  -- Confirm or Reroll";
                    hudResult.color = colName == "red"   ? new Color(1f, 0.6f, 0.6f) :
                                      colName == "black" ? Color.white :
                                                           new Color(0.6f, 1f, 0.6f);
                }
            }
            else if (postBet)
            {
                if (game.lastBetWon)
                {
                    // Check for multi-ball scoring
                    int scoringCount = 0;
                    if (game.ballResults != null)
                        foreach (var br in game.ballResults)
                            if (br.Scores) scoringCount++;

                    if (scoringCount > 1)
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.Append("WIN! Scoring balls: ");
                        bool first = true;
                        foreach (var br in game.ballResults)
                        {
                            if (!br.Scores) continue;
                            if (!first) sb.Append(", ");
                            first = false;
                            var bdef = ItemRegistry.Balls[br.Ball.Id];
                            sb.Append($"{bdef.Name}->{fxResolve.ResolveNumber(br.RolledNumber)}");
                        }
                        sb.Append($" -- +{game.lastPayout} to quota.");
                        hudResult.text = sb.ToString();
                    }
                    else
                    {
                        hudResult.text = $"Rolled {lastShown}. WIN via {game.lastWinCategory} -- +{game.lastPayout} to quota.";
                    }
                    hudResult.color = new Color(0.7f, 1f, 0.7f);
                    HighlightWinningZones(game.lastRolledNumber, game.lastWinCategory);
                    ShowBreakdown();
                }
                else
                {
                    hudResult.text  = $"Rolled {lastShown}. Miss -- penalty -1g from reward.";
                    hudResult.color = new Color(1f, 0.7f, 0.7f);
                    HideBreakdown();
                }
            }
            else if (shopping)
            {
                hudResult.text  = $"Quota cleared! Visit the shop, then press Continue for quota {game.CurrentQuota}.";
                hudResult.color = new Color(1f, 0.9f, 0.5f);
                HideBreakdown();
            }
            else if (gameOver)
            {
                hudResult.text  = $"GAME OVER -- couldn't make quota {game.CurrentQuota}. Press Restart.";
                hudResult.color = new Color(1f, 0.5f, 0.5f);
                HideBreakdown();
            }
            else if (betting)
            {
                hudResult.text  = "Place chips on numbers (left-click +25, right-click -25). Then Confirm Bet.";
                hudResult.color = Color.white;
                HideBreakdown();
            }
        }

        static void SetInteractable(Button b, bool v) { if (b) b.interactable = v; }

        static void SetStake(Text t, int amount)
        {
            if (t == null) return;
            t.text = amount > 0 ? "$" + amount : "";
        }

        static void SetMultLabel(Text t, string label)
        {
            if (t != null) t.text = label;
        }

        void HighlightWinningZones(int rolled, BetCategory bestCategory)
        {
            // Highlight the rolled number cell
            var cell = FindCell(rolled);
            bool numberIsBest = bestCategory == BetCategory.Number;
            if (cell != null && cell.highlight)
                cell.highlight.color = numberIsBest ? HI_BEST : HI;

            if (rolled == 0)
            {
                // Zero: highlight the number cell with best color if Zero is best category
                if (cell != null && cell.highlight && bestCategory == BetCategory.Zero)
                    cell.highlight.color = HI_BEST;
                return;
            }

            int doz = RouletteData.DozenOf(rolled);
            var col = RouletteData.ColorOf(rolled);

            bool dozIsBest   = bestCategory == BetCategory.Dozen;
            bool colorIsBest = bestCategory == BetCategory.Color;

            if (dozenHi != null && doz >= 1 && doz <= dozenHi.Length && dozenHi[doz - 1])
                dozenHi[doz - 1].color = dozIsBest ? HI_BEST : HI;
            if (col == BetColor.Red && redHi)
                redHi.color = colorIsBest ? HI_BEST : HI;
            if (col == BetColor.Black && blackHi)
                blackHi.color = colorIsBest ? HI_BEST : HI;
        }

        // ==============================================================
        //  Multiplier breakdown panel
        // ==============================================================

        void ShowBreakdown()
        {
            if (breakdownPanel == null || breakdownText == null) return;
            if (!game.lastBetWon) { HideBreakdown(); return; }

            var fx = new EffectProcessor(game.inventory);
            var sb = new System.Text.StringBuilder();

            // Collect scoring balls
            var scoringBalls = new System.Collections.Generic.List<BallRollResult>();
            if (game.ballResults != null)
                foreach (var br in game.ballResults)
                    if (br.Scores) scoringBalls.Add(br);

            bool multiBall = scoringBalls.Count > 1;

            if (multiBall)
            {
                // Multi-ball breakdown: show each ball's contribution separately
                sb.AppendLine("=== Multi-Ball Scoring ===");
                int grandTotal = 0;

                foreach (var br in scoringBalls)
                {
                    int rolled = br.RolledNumber;
                    string ballName = ItemRegistry.Balls[br.Ball.Id].Name;
                    var col = RouletteData.ColorOf(rolled);

                    // Find best category for this ball's rolled number
                    int ballPayout = 0;
                    BetCategory ballBestCat = BetCategory.Color;
                    int bestMult = -1;
                    foreach (var kvp in game.currentBet)
                    {
                        var cat = fx.BestCategoryFor(kvp.Key, rolled);
                        if (cat.HasValue)
                        {
                            var singleBall = new System.Collections.Generic.List<BallRollResult> { br };
                            int mult = fx.ComputeMultiplier(cat.Value, rolled, game.currentBet, game.goldChips, singleBall);
                            ballPayout += kvp.Value * mult;
                            if (mult > bestMult) { bestMult = mult; ballBestCat = cat.Value; }
                        }
                    }

                    if (bestMult < 0) continue; // ball didn't hit any bet

                    sb.AppendLine($"\n--- {ballName} -> {rolled} ({col}) ---");
                    int baseMult = RouletteData.MultiplierFor(ballBestCat);
                    sb.AppendLine($"  Base ({ballBestCat}): x{baseMult}");
                    int runningMult = baseMult;

                    // Card bonus
                    int cardBonus = fx.GetCategoryBonus(ballBestCat);
                    if (cardBonus > 0)
                    {
                        runningMult += cardBonus;
                        sb.AppendLine($"  Cards: +{cardBonus}  = x{runningMult}");
                    }

                    // Purple bonus (+10 to number category)
                    if (ballBestCat == BetCategory.Number && fx.GetNumberColor(rolled) == NumberColor.Purple)
                    {
                        runningMult += 10;
                        sb.AppendLine($"  Purple: +10  = x{runningMult}");
                    }

                    // Chip contributions
                    AppendChipContributions(sb, fx, rolled, game.currentBet, game.goldChips, game.inventory, ref runningMult);

                    // This ball's own bonus
                    if (br.AdditiveBonus != 0)
                    {
                        runningMult += br.AdditiveBonus;
                        sb.AppendLine($"  {ballName}: +{br.AdditiveBonus}  = x{runningMult}");
                    }
                    if (br.MultiplyBy > 1)
                    {
                        runningMult *= br.MultiplyBy;
                        sb.AppendLine($"  {ballName}: x{br.MultiplyBy}  = x{runningMult}");
                    }

                    sb.AppendLine($"  = {ballPayout}");
                    grandTotal += ballPayout;
                }

                sb.AppendLine($"\nTotal payout: {grandTotal}");
            }
            else
            {
                // Single ball / no ball breakdown
                int rolled = game.lastRolledNumber;
                var bestCat = game.lastWinCategory;
                int baseMult = RouletteData.MultiplierFor(bestCat);

                sb.AppendLine($"{game.TotalStake}  x  mult");
                sb.AppendLine($"Base ({bestCat}): x{baseMult}");

                int runningMult = baseMult;

                // Card bonus
                int cardBonus = fx.GetCategoryBonus(bestCat);
                if (cardBonus > 0)
                {
                    runningMult += cardBonus;
                    sb.AppendLine($"  Cards: +{cardBonus}  = x{runningMult}");
                }

                // Purple bonus (+10 to number category)
                if (bestCat == BetCategory.Number && fx.GetNumberColor(rolled) == NumberColor.Purple)
                {
                    runningMult += 10;
                    sb.AppendLine($"  Purple: +10  = x{runningMult}");
                }

                // Chip contributions
                AppendChipContributions(sb, fx, rolled, game.currentBet, game.goldChips, game.inventory, ref runningMult);

                // Ball contributions
                if (game.ballResults != null)
                {
                    foreach (var br in game.ballResults)
                    {
                        if (!br.Scores) continue;
                        if (br.AdditiveBonus != 0)
                        {
                            runningMult += br.AdditiveBonus;
                            string ballName = ItemRegistry.Balls[br.Ball.Id].Name;
                            sb.AppendLine($"  {ballName}: +{br.AdditiveBonus}  = x{runningMult}");
                        }
                        if (br.MultiplyBy > 1)
                        {
                            runningMult *= br.MultiplyBy;
                            string ballName = ItemRegistry.Balls[br.Ball.Id].Name;
                            sb.AppendLine($"  {ballName}: x{br.MultiplyBy}  = x{runningMult}");
                        }
                    }
                }

                sb.AppendLine();
                sb.AppendLine($"= {game.TotalStake} x {runningMult} = +{game.lastPayout} to quota");
            }

            breakdownText.text = sb.ToString();
            breakdownPanel.SetActive(true);
        }

        void AppendChipContributions(System.Text.StringBuilder sb, EffectProcessor fx, int rolled,
            System.Collections.Generic.Dictionary<int, int> bet, int goldChips, PlayerInventory inv, ref int runningMult)
        {
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                int add = GetChipAdditiveForDisplay(c, rolled, bet, goldChips, inv);
                int mul = GetChipMultiplicativeForDisplay(c, rolled, bet);
                if (add != 0)
                {
                    runningMult += add;
                    string chipName = ItemRegistry.Chips[c.Id].Name;
                    sb.AppendLine($"  {chipName}: +{add}  = x{runningMult}");
                }
                if (mul > 1)
                {
                    runningMult *= mul;
                    string chipName = ItemRegistry.Chips[c.Id].Name;
                    sb.AppendLine($"  {chipName}: x{mul}  = x{runningMult}");
                }
            }
        }

        void HideBreakdown()
        {
            if (breakdownPanel != null) breakdownPanel.SetActive(false);
        }

        // Display-only chip bonus extraction (mirrors EffectProcessor.ComputeMultiplier additive/multiplicative)
        static int GetChipAdditiveForDisplay(ChipInstance c, int rolled, Dictionary<int, int> bet, int goldChips, PlayerInventory inv)
        {
            switch (c.Id)
            {
                case ChipId.Bordeaux:   return (rolled > 0 && RouletteData.ColorOf(rolled) == BetColor.Red) ? 1 : 0;
                case ChipId.Dark:       return (rolled > 0 && RouletteData.ColorOf(rolled) == BetColor.Black) ? 1 : 0;
                case ChipId.BlackDozen: return (rolled > 0 && RouletteData.DozenOf(rolled) == c.TargetGroup) ? 2 : 0;
                case ChipId.Seven:
                    if (c.TargetNumbers != null)
                        foreach (int tn in c.TargetNumbers)
                            if (tn == rolled) return 4;
                    return 0;
                case ChipId.Zero:       return rolled == 0 ? 10 : 0;
                case ChipId.Even:       return (rolled > 0 && rolled % 2 == 0) ? 2 : 0;
                case ChipId.Odd:        return (rolled > 0 && rolled % 2 == 1) ? 2 : 0;
                case ChipId.Prime:
                    var primes = new HashSet<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };
                    return primes.Contains(rolled) ? 2 : 0;
                case ChipId.High:       return rolled > 18 ? 2 : 0;
                case ChipId.Low:        return (rolled > 0 && rolled < 18) ? 2 : 0;
                case ChipId.Clover:     return 2;
                case ChipId.Coin:       return 2;
                case ChipId.GoldBar:    return goldChips;
                case ChipId.Dots:       return inv.ActiveChipCount - 1;
                case ChipId.X:          return 1;
                case ChipId.BallChip:   return inv.ActiveBallCount;
                case ChipId.AntiBall:   return 3;
                case ChipId.D6:         return 3;
                case ChipId.RedBank:    return c.AccumulatedBonus;
                case ChipId.BlackBank:  return c.AccumulatedBonus;
                case ChipId.Holey:
                    var fx2 = new EffectProcessor(inv);
                    int empty = Mathf.Max(0, fx2.GetChipSlots() - inv.ActiveChipCount);
                    return 2 * empty;
                default: return 0;
            }
        }

        static int GetChipMultiplicativeForDisplay(ChipInstance c, int rolled, Dictionary<int, int> bet)
        {
            switch (c.Id)
            {
                case ChipId.Loss:   return 2;
                case ChipId.VaBank: return (bet != null && bet.Count == 1) ? 3 : 1;
                case ChipId.Luck4:  return (rolled > 0 && rolled % 4 == 0) ? 2 : 1;
                default: return 1;
            }
        }

        NumberCellRef FindCell(int n)
        {
            if (numberCells == null) return null;
            foreach (var c in numberCells) if (c != null && c.number == n) return c;
            return null;
        }

        // ==============================================================
        //  Bet popup (click a number to set its stake via input field)
        // ==============================================================

        GameObject betPopup;
        Text betPopupTitle;
        Text betPopupBalanceText;
        InputField betAmountInput;
        int betPopupNumber;

        void ShowBetPopup(int number)
        {
            if (game == null) return;
            if (game.Phase != GamePhase.Betting) return;
            EnsureBetPopup();
            if (betPopup == null) return;

            betPopupNumber = number;
            if (betPopupTitle != null) betPopupTitle.text = "Bet on " + number;

            int existing = game.currentBet.TryGetValue(number, out var v) ? v : 0;
            if (betAmountInput != null)
                betAmountInput.text = existing > 0 ? existing.ToString() : "";

            if (betPopupBalanceText != null)
            {
                int available = game.baseMoney - (game.TotalStake - existing);
                if (available < 0) available = 0;
                betPopupBalanceText.text = $"Available: ${available}   (step ${RouletteGame.MinBetStep})";
            }

            betPopup.SetActive(true);
            betPopup.transform.SetAsLastSibling();
            if (betAmountInput != null)
            {
                betAmountInput.Select();
                betAmountInput.ActivateInputField();
            }
        }

        void OnBetPopupConfirm()
        {
            if (game == null) return;
            int amount = 0;
            if (betAmountInput != null && !string.IsNullOrEmpty(betAmountInput.text))
                int.TryParse(betAmountInput.text, out amount);
            game.SetBetAmount(betPopupNumber, amount);
            if (betPopup != null) betPopup.SetActive(false);
        }

        void OnBetPopupCancel()
        {
            if (betPopup != null) betPopup.SetActive(false);
        }

        void EnsureBetPopup()
        {
            if (betPopup != null) return;

            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
#if UNITY_2022_2_OR_NEWER
                canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
#else
                canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
#endif
            }
            if (canvas == null) return;

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Root panel
            betPopup = new GameObject("BetPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            betPopup.transform.SetParent(canvas.transform, false);
            betPopup.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.97f);
            var rt = betPopup.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(520, 340);
            rt.anchoredPosition = Vector2.zero;

            // Title
            var titleGo = CreatePopupText(betPopup.transform, "Title", "Bet on --", 40, Color.white, font);
            var titleRT = titleGo.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 1);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot     = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -20);
            titleRT.sizeDelta = new Vector2(-40, 60);
            titleGo.GetComponent<Text>().fontStyle = FontStyle.Bold;
            betPopupTitle = titleGo.GetComponent<Text>();

            // Balance helper text
            var balGo = CreatePopupText(betPopup.transform, "Balance", "", 22, new Color(0.75f, 0.75f, 0.8f, 1f), font);
            var balRT = balGo.GetComponent<RectTransform>();
            balRT.anchorMin = new Vector2(0, 1);
            balRT.anchorMax = new Vector2(1, 1);
            balRT.pivot     = new Vector2(0.5f, 1);
            balRT.anchoredPosition = new Vector2(0, -90);
            balRT.sizeDelta = new Vector2(-40, 32);
            betPopupBalanceText = balGo.GetComponent<Text>();

            // Amount input field (legacy InputField)
            var inputGo = new GameObject("Amount", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            inputGo.transform.SetParent(betPopup.transform, false);
            inputGo.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.28f, 1f);
            var inputRT = inputGo.GetComponent<RectTransform>();
            inputRT.anchorMin = new Vector2(0.5f, 1);
            inputRT.anchorMax = new Vector2(0.5f, 1);
            inputRT.pivot     = new Vector2(0.5f, 1);
            inputRT.anchoredPosition = new Vector2(0, -140);
            inputRT.sizeDelta = new Vector2(340, 80);

            var textGo = CreatePopupText(inputGo.transform, "Text", "", 44, Color.white, font);
            var textRT = textGo.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(14, 6);
            textRT.offsetMax = new Vector2(-14, -6);

            var phGo = CreatePopupText(inputGo.transform, "Placeholder", "0", 44, new Color(0.55f, 0.55f, 0.6f, 0.8f), font);
            phGo.GetComponent<Text>().fontStyle = FontStyle.Italic;
            var phRT = phGo.GetComponent<RectTransform>();
            phRT.anchorMin = Vector2.zero;
            phRT.anchorMax = Vector2.one;
            phRT.offsetMin = new Vector2(14, 6);
            phRT.offsetMax = new Vector2(-14, -6);

            var inputField = inputGo.AddComponent<InputField>();
            inputField.textComponent = textGo.GetComponent<Text>();
            inputField.placeholder   = phGo.GetComponent<Text>();
            inputField.contentType   = InputField.ContentType.IntegerNumber;
            inputField.characterLimit = 7;
            betAmountInput = inputField;

            // Confirm button
            var confirmGo = CreatePopupButton(betPopup.transform, "ConfirmBtn", "CONFIRM", new Color(0.2f, 0.6f, 0.3f, 1f), font);
            var cfRT = confirmGo.GetComponent<RectTransform>();
            cfRT.anchorMin = new Vector2(0.5f, 0);
            cfRT.anchorMax = new Vector2(0.5f, 0);
            cfRT.pivot     = new Vector2(1f, 0f);
            cfRT.anchoredPosition = new Vector2(-10, 30);
            cfRT.sizeDelta = new Vector2(230, 80);
            confirmGo.GetComponent<Button>().onClick.AddListener(OnBetPopupConfirm);

            // Cancel button
            var cancelGo = CreatePopupButton(betPopup.transform, "CancelBtn", "CANCEL", new Color(0.55f, 0.2f, 0.2f, 1f), font);
            var caRT = cancelGo.GetComponent<RectTransform>();
            caRT.anchorMin = new Vector2(0.5f, 0);
            caRT.anchorMax = new Vector2(0.5f, 0);
            caRT.pivot     = new Vector2(0f, 0f);
            caRT.anchoredPosition = new Vector2(10, 30);
            caRT.sizeDelta = new Vector2(230, 80);
            cancelGo.GetComponent<Button>().onClick.AddListener(OnBetPopupCancel);

            betPopup.SetActive(false);
        }

        static GameObject CreatePopupText(Transform parent, string name, string content, int size, Color color, Font font)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = font;
            t.fontSize = size;
            t.color = color;
            t.text = content;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return go;
        }

        static GameObject CreatePopupButton(Transform parent, string name, string label, Color color, Font font)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            var labelGo = CreatePopupText(go.transform, "Label", label, 34, Color.white, font);
            labelGo.GetComponent<Text>().fontStyle = FontStyle.Bold;
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            return go;
        }
    }

    public class RightClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public Action OnRightClick;
        public void OnPointerClick(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right) OnRightClick?.Invoke();
        }
    }
}
