using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadlyRoulette
{
    public enum GamePhase
    {
        Betting,            // player placing chips
        Spinning,           // wheel rolling (instant in prototype)
        ResultPending,      // results shown, player selects ball then confirms/rerolls
        PostBet,            // after confirm: continue or end quota
        PostBossUpgrade,    // post-boss reward screen (between PostBet and Shopping)
        Shopping,           // quota settled, shop is open, waiting for Continue
        GameOver
    }

    public class RouletteGame : MonoBehaviour
    {
        public const int MinBetStep = 25;
        public const int BaseBankroll = 200;
        public const int BetsPerQuota = 3;
        public const int RerollsPerQuota = 5;
        public const int QuotaBaseReward = 3;
        public const int UnusedBetBonus = 2;
        // Reserved for a future chip — base gameplay no longer awards a flat reroll bonus.
        public const int UnusedRerollBonus = 1;
        public const int MaxOvershootBonus = 9;

        public static readonly int[] QuotaProgression = { 400, 800, 1600, 2000, 4000, 8000, 10000, 20000, 40000, 50000, 100000, 200000 };

        /// <summary>True when the quota at the given index is a boss (every 3rd quota: indices 2, 5, 8, 11).</summary>
        public static bool IsBossQuota(int index) => (index + 1) % 3 == 0;

        public bool CurrentIsBoss => IsBossQuota(quotaIndex);

        // persistent
        public int goldChips;
        public int quotaIndex;
        public PlayerInventory inventory = new PlayerInventory();

        // per quota
        public int baseMoney;           // fixed bankroll for betting (doesn't change during quota)
        public int quotaProgress;       // accumulated winnings toward the quota
        public int goldPenalty;          // gold chip fines from lost bets (subtracted from reward)
        public int betsRemaining;
        public int rerollsRemaining;

        // per bet
        public Dictionary<int, int> currentBet = new Dictionary<int, int>(); // number -> amount
        public int lastRolledNumber;        // the selected ball's number (for backward compat)
        public int lastPayout;
        public BetCategory lastWinCategory;
        public bool lastBetWon;

        // multi-ball results (set after each spin)
        public List<BallRollResult> ballResults = new List<BallRollResult>();
        public int selectedBallIndex;

        // spin effects (set after each spin)
        public SpinEffects lastSpinEffects;

        public GamePhase Phase { get; private set; } = GamePhase.Betting;

        public int CurrentQuota
        {
            get
            {
                int baseQuota = QuotaProgression[Mathf.Min(quotaIndex, QuotaProgression.Length - 1)];
                return fx != null ? fx.GetModifiedQuota(baseQuota) : baseQuota;
            }
        }

        public int TotalStake
        {
            get { int s = 0; foreach (var v in currentBet.Values) s += v; return s; }
        }

        public event Action OnStateChanged;

        System.Random rng = new System.Random();
        EffectProcessor fx;

        public EffectProcessor Fx => fx;

        void Start()
        {
            fx = new EffectProcessor(inventory, rng);
            // Start with one normal ball if none exist
            if (inventory.ownedBalls.Count == 0)
                inventory.ownedBalls.Add(new BallInstance { Id = BallId.Normal });
            StartNewQuota(true);
        }

        void EnsureFx()
        {
            if (fx == null) fx = new EffectProcessor(inventory, rng);
        }

        // ==============================================================
        //  Quota / Bet lifecycle
        // ==============================================================

        void StartNewQuota(bool first)
        {
            EnsureFx();
            baseMoney = fx.GetBaseBankroll();
            quotaProgress = 0;
            goldPenalty = 0;
            betsRemaining = fx.GetBetsPerQuota();
            rerollsRemaining = fx.GetRerollsPerQuota();
            fx.ResetQuotaState();
            BeginNewBet();
        }

        void BeginNewBet()
        {
            EnsureFx();
            currentBet.Clear();
            ballResults.Clear();
            selectedBallIndex = 0;
            lastPayout = 0;
            lastBetWon = false;
            fx.ResetBetState();
            fx.OnBetStarted();
            Phase = GamePhase.Betting;
            Notify();
        }

        // ==============================================================
        //  Betting
        // ==============================================================

        public void AddChip(int number)
        {
            if (Phase != GamePhase.Betting) return;
            if (number < 0 || number > 36) return;
            if (TotalStake + MinBetStep > baseMoney) return;

            // Enforce max-bet-targets rule: clicking a NEW number when at the cap
            // wipes existing stakes and starts fresh on the new cell.
            EnsureFx();
            if (fx.IsNumberRemoved(number)) return;
            int maxTargets = fx.GetMaxBetNumbers();
            if (!currentBet.ContainsKey(number) && currentBet.Count >= maxTargets)
                currentBet.Clear();

            if (!currentBet.ContainsKey(number)) currentBet[number] = 0;
            currentBet[number] += MinBetStep;
            Notify();
        }

        public void RemoveChip(int number)
        {
            if (Phase != GamePhase.Betting) return;
            if (!currentBet.ContainsKey(number)) return;
            currentBet[number] -= MinBetStep;
            if (currentBet[number] <= 0) currentBet.Remove(number);
            Notify();
        }

        public void ClearBet()
        {
            if (Phase != GamePhase.Betting) return;
            currentBet.Clear();
            Notify();
        }

        /// <summary>Set the bet stake on a specific number directly (replaces current stake on it).
        /// Clamps to multiples of MinBetStep and to the base money limit.</summary>
        public void SetBetAmount(int number, int amount)
        {
            if (Phase != GamePhase.Betting) return;
            if (number < 0 || number > 36) return;
            if (amount < 0) amount = 0;

            // Snap down to MinBetStep multiple
            amount = (amount / MinBetStep) * MinBetStep;

            // Single-target rule: setting a non-zero stake on a new number
            // wipes any stake on other numbers first.
            EnsureFx();
            if (amount > 0 && fx.IsNumberRemoved(number)) return;
            int maxTargets = fx.GetMaxBetNumbers();
            if (amount > 0 && !currentBet.ContainsKey(number) && currentBet.Count >= maxTargets)
                currentBet.Clear();

            int existing = currentBet.TryGetValue(number, out var v) ? v : 0;
            int stakeWithoutThis = TotalStake - existing;
            int maxAffordable = baseMoney - stakeWithoutThis;
            if (maxAffordable < 0) maxAffordable = 0;
            if (amount > maxAffordable)
                amount = (maxAffordable / MinBetStep) * MinBetStep;

            if (amount <= 0) currentBet.Remove(number);
            else currentBet[number] = amount;
            Notify();
        }

        /// <summary>Reorder a chip in the inventory (drag-and-drop).</summary>
        public void ReorderChip(int fromIndex, int toIndex)
        {
            if (Phase != GamePhase.Betting) return;
            if (fromIndex < 0 || fromIndex >= inventory.equippedChips.Count) return;
            if (toIndex < 0 || toIndex >= inventory.equippedChips.Count) return;
            if (fromIndex == toIndex) return;
            var chip = inventory.equippedChips[fromIndex];
            inventory.equippedChips.RemoveAt(fromIndex);
            inventory.equippedChips.Insert(toIndex, chip);
            Notify();
        }

        public void ConfirmBetAndSpin()
        {
            if (Phase != GamePhase.Betting) return;
            if (TotalStake <= 0) return;
            Spin();
        }

        // ==============================================================
        //  Spinning
        // ==============================================================

        void Spin()
        {
            EnsureFx();
            Phase = GamePhase.Spinning;

            // Roll all balls
            ballResults = fx.RollBalls(selectedBallIndex);

            if (ballResults.Count > 0)
            {
                // Default: select first scoring ball
                selectedBallIndex = 0;
                for (int i = 0; i < ballResults.Count; i++)
                {
                    if (ballResults[i].Scores) { selectedBallIndex = i; break; }
                }
                lastRolledNumber = ballResults[selectedBallIndex].RolledNumber;
            }
            else
            {
                // No balls — fallback to classic single roll (respect Boom/BigBang removals).
                lastRolledNumber = fx.RollFromPool();
            }

            // Process spin effects (board triggers: gray, white, blue, yellow, orange)
            lastSpinEffects = fx.ProcessSpinResult(lastRolledNumber, CurrentQuota);

            // Update bank chip accumulators
            fx.OnNumberRolled(lastRolledNumber);

            // Apply instant effects
            if (lastSpinEffects.GoldChipsGained > 0)
                goldChips += lastSpinEffects.GoldChipsGained;
            if (lastSpinEffects.RerollsAdded > 0)
                rerollsRemaining += lastSpinEffects.RerollsAdded;

            // Auto-reroll (blue, anti-red, anti-black, crossed chips)
            if (lastSpinEffects.AutoReroll)
            {
                // Free reroll — don't consume a reroll charge
                Spin();
                return;
            }

            // Instant loss (gray number) — gold penalty, no money loss
            if (lastSpinEffects.InstantLoss)
            {
                goldPenalty++;
                lastPayout = 0;
                lastBetWon = false;
                betsRemaining--;
                inventory.betsUsedThisQuota++;
                Phase = GamePhase.PostBet;
                CheckGameOver();
                Notify();
                return;
            }

            // White number: instantly fill quota progress
            if (lastSpinEffects.SetBalanceToQuota)
            {
                quotaProgress = CurrentQuota;
                lastPayout = 0;
                lastBetWon = false;
                betsRemaining--;
                inventory.betsUsedThisQuota++;

                // Destroy white chip
                foreach (var c in lastSpinEffects.ChipsToDestroy)
                    c.Destroyed = true;

                Phase = GamePhase.PostBet;
                Notify();
                return;
            }

            Phase = GamePhase.ResultPending;
            Notify();
        }

        // ==============================================================
        //  Ball selection (during ResultPending)
        // ==============================================================

        /// <summary>Player selects which ball result to use. Any ball may be chosen
        /// (even non-scoring ones); the scoring flags are re-derived so the newly
        /// chosen ball becomes the scoring one. Stone balls always stay scoring.</summary>
        public void SelectBall(int index)
        {
            if (Phase != GamePhase.ResultPending) return;
            if (index < 0 || index >= ballResults.Count) return;

            selectedBallIndex = index;
            lastRolledNumber = ballResults[index].RolledNumber;

            // Re-derive Scores flags based on the new chosen index.
            int choices = fx != null ? fx.GetBallChoices() : 1;
            bool randomChoice = fx != null && fx.IsRandomBallChoice();
            for (int i = 0; i < ballResults.Count; i++)
            {
                var br = ballResults[i];
                bool isStone = br.Ball != null && br.Ball.Id == BallId.Stone;
                bool isChosen = false;
                if (!randomChoice)
                {
                    for (int c = 0; c < choices; c++)
                    {
                        if ((selectedBallIndex + c) % ballResults.Count == i) { isChosen = true; break; }
                    }
                }
                else
                {
                    // D6 randomization happens at spin time; keep whatever was rolled
                    isChosen = br.Scores && !isStone;
                }
                br.Scores = isStone || isChosen;
                ballResults[i] = br;
            }

            Notify();
        }

        // ==============================================================
        //  Reroll / Confirm
        // ==============================================================

        public void Reroll()
        {
            if (Phase != GamePhase.ResultPending) return;
            if (rerollsRemaining <= 0) return;
            rerollsRemaining--;
            Spin();
        }

        public void ConfirmResult()
        {
            if (Phase != GamePhase.ResultPending) return;

            EnsureFx();
            int rolled = lastRolledNumber;

            // Gather scoring balls for multiplier bonuses
            var scoringBalls = new List<BallRollResult>();
            foreach (var br in ballResults)
                if (br.Scores) scoringBalls.Add(br);

            // Expose per-bet context to ComputeMultiplier (LastChance, EncoreMore, RerollReward).
            fx.CurrentRerollsRemaining = rerollsRemaining;
            fx.CurrentBetsRemaining    = betsRemaining;

            // Compute payout using EffectProcessor (handles all chip/card/ball bonuses)
            int totalPayout = fx.ComputeTotalPayout(
                rolled, currentBet, goldChips, scoringBalls,
                out BetCategory bestCat, out bool anyWin);

            if (anyWin)
            {
                // Winnings go to quota progress
                quotaProgress += totalPayout;
                // LuckyNumber: count wins on the Number category
                if (bestCat == BetCategory.Number)
                    fx.OnNumberBetWon();
            }
            else
            {
                // Lost bet: 1 gold chip penalty from quota reward
                goldPenalty++;
            }

            lastPayout = totalPayout;
            lastBetWon = anyWin;
            lastWinCategory = bestCat;

            // Budget chip: +50 base money per bet used this quota.
            inventory.betsUsedThisQuota++;

            // Destroy chips/balls from spin effects and ball effects.
            // Only destroy balls that were actually scoring — otherwise non-chosen
            // Glass/Explosive rolls would punish players who didn't pick them.
            foreach (var c in lastSpinEffects.ChipsToDestroy)
                c.Destroyed = true;
            foreach (var br in ballResults)
                if (br.ShouldDestroy && br.Scores && br.Ball != null)
                    br.Ball.Destroyed = true;

            betsRemaining--;
            Phase = GamePhase.PostBet;
            CheckGameOver();
            Notify();
        }

        // ==============================================================
        //  Post-bet
        // ==============================================================

        void CheckGameOver()
        {
            bool canEndQuota = quotaProgress >= CurrentQuota;
            bool canKeepBetting = betsRemaining > 0;

            if (!canEndQuota && !canKeepBetting)
            {
                // Skull chip save
                if (fx != null && fx.TrySkullSave())
                {
                    SettleQuotaForced();
                    return;
                }
                Phase = GamePhase.GameOver;
            }
        }

        public void ContinueAfterBet()
        {
            if (Phase != GamePhase.PostBet) return;
            if (betsRemaining <= 0)
            {
                SettleQuota();
                return;
            }
            BeginNewBet();
        }

        public bool CanEndQuota => Phase == GamePhase.PostBet && quotaProgress >= CurrentQuota;

        public void EndQuota()
        {
            if (Phase != GamePhase.PostBet) return;
            if (quotaProgress < CurrentQuota) return;
            SettleQuota();
        }

        // ==============================================================
        //  Quota settlement
        // ==============================================================

        void SettleQuota()
        {
            EnsureFx();
            if (quotaProgress >= CurrentQuota)
            {
                // RerollReward reads this when GetBaseReward runs.
                fx.CurrentRerollsRemaining = rerollsRemaining;

                int unusedBets = Mathf.Max(0, betsRemaining);
                int reward = fx.GetBaseReward()
                           + unusedBets * fx.GetUnusedBetBonus()
                           + ComputeOvershootBonus(quotaProgress, CurrentQuota);
                // Apply gold penalty from lost bets
                reward = Mathf.Max(0, reward - goldPenalty);
                goldChips += reward;

                // Run-scoped accumulators.
                inventory.unusedRerollsRun += Mathf.Max(0, rerollsRemaining);
                fx.OnQuotaSettled(quotaProgress, CurrentQuota);

                AdvanceQuotaAndRouteToNextPhase();
            }
            else
            {
                if (fx.TrySkullSave())
                {
                    SettleQuotaForced();
                    return;
                }
                Phase = GamePhase.GameOver;
                Notify();
            }
        }

        /// <summary>Skull chip forced quota pass — gives base reward only (still applies penalty).</summary>
        void SettleQuotaForced()
        {
            EnsureFx();
            fx.CurrentRerollsRemaining = rerollsRemaining;
            int reward = Mathf.Max(0, fx.GetBaseReward() - goldPenalty);
            goldChips += reward;
            inventory.unusedRerollsRun += Mathf.Max(0, rerollsRemaining);
            fx.OnQuotaSettled(quotaProgress, CurrentQuota);
            AdvanceQuotaAndRouteToNextPhase();
        }

        /// <summary>x2 → 1, x3 → 2, ..., x10 → 9. Capped at MaxOvershootBonus.</summary>
        static int ComputeOvershootBonus(int progress, int quota)
        {
            if (quota <= 0) return 0;
            int multiple = progress / quota;
            if (multiple < 2) return 0;
            return Mathf.Min(MaxOvershootBonus, multiple - 1);
        }

        void AdvanceQuotaAndRouteToNextPhase()
        {
            bool wasBoss = CurrentIsBoss;
            if (wasBoss) inventory.bossChips++;
            quotaIndex = Mathf.Min(quotaIndex + 1, QuotaProgression.Length - 1);
            Phase = wasBoss ? GamePhase.PostBossUpgrade : GamePhase.Shopping;
            Notify();
        }

        /// <summary>Called by PostBossUI when the player finishes claiming post-boss rewards.</summary>
        public void ContinueFromPostBoss()
        {
            if (Phase != GamePhase.PostBossUpgrade) return;
            Phase = GamePhase.Shopping;
            Notify();
        }

        /// <summary>Called by ShopUI Continue button to leave shop and begin next quota.</summary>
        public void ContinueFromShop()
        {
            if (Phase != GamePhase.Shopping) return;
            StartNewQuota(false);
        }

        public void RestartFromGameOver()
        {
            if (Phase != GamePhase.GameOver) return;
            quotaIndex = 0;
            // Reset inventory state but keep items
            inventory.ResetQuotaState();
            inventory.quotasBeatenDouble = 0;
            inventory.unusedRerollsRun = 0;
            inventory.wonNumberBets = 0;
            inventory.bossChips = 0;
            inventory.plaks.Clear();
#if UNITY_2022_2_OR_NEWER
            var shop = UnityEngine.Object.FindFirstObjectByType<ShopManager>();
#else
            var shop = UnityEngine.Object.FindObjectOfType<ShopManager>();
#endif
            if (shop != null) shop.baseMoneyUpgradesPurchased = 0;
            StartNewQuota(true);
        }

        void Notify() => OnStateChanged?.Invoke();
    }
}
