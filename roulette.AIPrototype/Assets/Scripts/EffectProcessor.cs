using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadlyRoulette
{
    /// <summary>
    /// Result of processing a spin — checked before the player confirms.
    /// </summary>
    public struct SpinEffects
    {
        public bool AutoReroll;         // a chip/ball triggered a free reroll
        public bool InstantLoss;        // gray number was rolled
        public bool SetBalanceToQuota;  // white number was rolled
        public bool ReturnStake;        // white number was rolled
        public int GoldChipsGained;     // yellow number hits
        public int RerollsAdded;        // orange number hits
        public List<ChipInstance> ChipsToDestroy;  // chips consumed by trigger
    }

    /// <summary>
    /// Result of a single ball's roll in a multi-ball spin.
    /// </summary>
    public struct BallRollResult
    {
        public BallInstance Ball;
        public int RolledNumber;
        public bool Scores;             // true if chosen or Stone
        public int AdditiveBonus;       // flat bonus from ball effect
        public int MultiplyBy;          // multiplicative bonus (1 = none)
        public bool ShouldDestroy;
    }

    /// <summary>
    /// Computes all item/upgrade effects. Stateful: tracks per-bet triggers.
    /// Create one instance and call ResetBetState() at the start of each bet.
    /// </summary>
    public class EffectProcessor
    {
        readonly PlayerInventory inv;
        readonly System.Random rng;

        // Per-bet trigger tracking (reset each bet)
        readonly HashSet<string> firedTriggers = new HashSet<string>();

        // Primes up to 36
        static readonly HashSet<int> Primes = new HashSet<int>
            { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };

        // Numbers targeted by the Nine chip (x2 mult)
        static readonly HashSet<int> NineNumbers = new HashSet<int>
            { 6, 9, 16, 19, 26, 29, 36 };

        // ---- Live game context (set by RouletteGame each bet) ----
        public int CurrentRerollsRemaining;     // for LastChance / RerollReward
        public int CurrentBetsRemaining;        // for EncoreMore (==1 on last bet)

        public EffectProcessor(PlayerInventory inventory, System.Random random = null)
        {
            inv = inventory;
            rng = random ?? new System.Random();
        }

        // ==============================================================
        //  Per-bet / per-quota lifecycle
        // ==============================================================

        public void ResetBetState()
        {
            firedTriggers.Clear();
        }

        public void ResetQuotaState()
        {
            inv.ResetQuotaState();
        }

        /// <summary>Called at the start of each bet to age magnet balls (destroyed after 5 bets).</summary>
        public void OnBetStarted()
        {
            foreach (var ball in inv.ownedBalls)
            {
                if (ball.Destroyed) continue;
                if (ball.Id == BallId.RedMagnet || ball.Id == BallId.BlackMagnet)
                {
                    ball.BetsUsed++;
                    if (ball.BetsUsed > 5)
                        ball.Destroyed = true;
                }
            }
        }

        /// <summary>Called after every roll (confirmed or not) to update bank chip accumulators.</summary>
        public void OnNumberRolled(int rolled)
        {
            var color = rolled > 0 ? RouletteData.ColorOf(rolled) : BetColor.None;
            foreach (var chip in inv.equippedChips)
            {
                if (chip.Destroyed) continue;
                if (chip.Id == ChipId.RedBank && color == BetColor.Red)
                    chip.AccumulatedBonus++;
                if (chip.Id == ChipId.BlackBank && color == BetColor.Black)
                    chip.AccumulatedBonus++;

                // PrimeBank: +3 per prime; resets on non-prime (0 counts as non-prime).
                if (chip.Id == ChipId.PrimeBank)
                {
                    if (Primes.Contains(rolled)) chip.AccumulatedBonus += 3;
                    else chip.AccumulatedBonus = 0;
                }

                // GreenBank: +4 per zero rolled; never resets.
                if (chip.Id == ChipId.GreenBank && rolled == 0)
                    chip.AccumulatedBonus += 4;
            }
        }

        /// <summary>Called at end of quota to update run-scoped accumulators.</summary>
        public void OnQuotaSettled(int quotaProgress, int quotaTarget)
        {
            // Champion: +1 to quotasBeatenDouble whenever progress >= 2x target.
            if (quotaTarget > 0 && quotaProgress >= quotaTarget * 2)
                inv.quotasBeatenDouble++;

            // AntiRoll: unused rerolls rolled over from this quota persist.
            // RouletteGame adds to inv.unusedRerollsRun when settling.
        }

        /// <summary>Called when the player wins a bet on a single number — LuckyNumber counter.</summary>
        public void OnNumberBetWon()
        {
            inv.wonNumberBets++;
        }

        // ==============================================================
        //  Game parameter queries
        // ==============================================================

        public int GetBetsPerQuota()
        {
            int bets = RouletteGame.BetsPerQuota + inv.bonusBetsPerQuota;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Stack) bets++;
                if (c.Id == ChipId.Coin)  bets--;
            }
            return Mathf.Max(1, bets);
        }

        public int GetRerollsPerQuota()
        {
            int rerolls = RouletteGame.RerollsPerQuota + inv.bonusRerollsPerQuota;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Reroll) rerolls += 2;
                if (c.Id == ChipId.Clover) rerolls -= 2;
            }
            return Mathf.Max(0, rerolls);
        }

        public int GetBaseBankroll()
        {
            int money = RouletteGame.BaseBankroll + inv.bonusBaseMoney;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Investment) money += 400;
                if (c.Id == ChipId.Budget)     money += 50 * inv.betsUsedThisQuota;
            }
            return money;
        }

        public int GetModifiedQuota(int baseQuota)
        {
            float quota = baseQuota;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Percent) quota *= 0.75f;
            }
            return Mathf.Max(1, Mathf.RoundToInt(quota));
        }

        public int GetBaseReward()
        {
            int reward = RouletteGame.QuotaBaseReward + inv.bonusBaseReward;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Mining)       reward += 2;
                if (c.Id == ChipId.RerollReward) reward += Mathf.Max(0, CurrentRerollsRemaining);
            }
            return reward;
        }

        public int GetUnusedBetBonus()
        {
            return RouletteGame.UnusedBetBonus + inv.bonusUnusedBetReward;
        }

        /// <summary>How many distinct numbers the player may stake on simultaneously.
        /// MultiBet adds +1, MultiBet+ adds +2 per equipped copy.</summary>
        public int GetMaxBetNumbers()
        {
            int max = 1;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.MultiBet)     max += 1;
                if (c.Id == ChipId.MultiBetPlus) max += 2;
            }
            return max;
        }

        public int GetChipSlots()
        {
            int slots = inv.baseChipSlots + inv.bonusChipSlots;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Empty) slots += 2;
                if (c.Id == ChipId.Loss)  slots--;
            }
            return Mathf.Max(1, slots);
        }

        public int GetBallSlots()
        {
            int slots = inv.baseBallSlots + inv.bonusBallSlots;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.EightBall) slots++;
                if (c.Id == ChipId.AntiBall)  slots--;
            }
            return Mathf.Max(0, slots);
        }

        public int GetBallChoices()
        {
            int choices = inv.baseBallChoices;
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;
                if (c.Id == ChipId.Checkmark) choices++;
            }
            return choices;
        }

        public bool IsRandomBallChoice()
        {
            return inv.HasChip(ChipId.D6);
        }

        // ==============================================================
        //  Board state queries
        // ==============================================================

        /// <summary>
        /// Returns the modified color of a number on the board based on plaks.
        /// White plaks remember triggered numbers and revert them to default.
        /// </summary>
        public NumberColor GetNumberColor(int number)
        {
            foreach (var p in inv.plaks)
            {
                if (p.TargetNumbers == null) continue;
                NumberColor? col = PlakRecolorTarget(p.Id);
                if (!col.HasValue) continue;
                for (int i = 0; i < p.TargetNumbers.Length; i++)
                {
                    if (p.TargetNumbers[i] != number) continue;
                    // White plak reverts that number to default after it has triggered.
                    if (p.Id == PlakId.White && p.TriggeredNumbers != null && p.TriggeredNumbers.Contains(number))
                        continue;
                    return col.Value;
                }
            }
            return NumberColor.Default;
        }

        static NumberColor? PlakRecolorTarget(PlakId id)
        {
            switch (id)
            {
                case PlakId.Black:  return NumberColor.Black;
                case PlakId.Red:    return NumberColor.Red;
                case PlakId.White:  return NumberColor.White;
                case PlakId.Blue:   return NumberColor.Blue;
                case PlakId.Gray:   return NumberColor.Gray;
                case PlakId.Purple: return NumberColor.Purple;
                case PlakId.Orange: return NumberColor.Orange;
                case PlakId.Yellow: return NumberColor.Yellow;
                default: return null;
            }
        }

        /// <summary>
        /// Returns true if the number has been removed from the board by a Crossed plak.
        /// </summary>
        public bool IsNumberRemoved(int number)
        {
            foreach (var p in inv.plaks)
            {
                if (p.Id != PlakId.Crossed || p.TargetNumbers == null) continue;
                for (int i = 0; i < p.TargetNumbers.Length; i++)
                    if (p.TargetNumbers[i] == number) return true;
            }
            return false;
        }

        /// <summary>
        /// Resolves what number a board position actually represents
        /// after Spikes chip or Green plak replacements.
        /// </summary>
        public int ResolveNumber(int boardPosition)
        {
            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed || c.TargetNumbers == null) continue;

                // Spikes: TargetNumbers[0] = chosen, [1..] = 3 replaced positions.
                if (c.Id == ChipId.Spikes && c.TargetNumbers.Length >= 2)
                {
                    int chosen = c.TargetNumbers[0];
                    for (int i = 1; i < c.TargetNumbers.Length; i++)
                        if (boardPosition == c.TargetNumbers[i]) return chosen;
                }
            }
            // Green plak: TargetNumbers[0] = chosen, [1..] = replaced positions.
            foreach (var p in inv.plaks)
            {
                if (p.Id != PlakId.Green || p.TargetNumbers == null || p.TargetNumbers.Length < 2) continue;
                int chosen = p.TargetNumbers[0];
                for (int i = 1; i < p.TargetNumbers.Length; i++)
                    if (boardPosition == p.TargetNumbers[i]) return chosen;
            }
            return boardPosition;
        }

        /// <summary>
        /// Checks if a bet on betNumber is hit by the rolled number,
        /// accounting for MoreLess, Glass, and OneThirtySix chips.
        /// </summary>
        public bool IsNumberHit(int betNumber, int rolledNumber)
        {
            int resolvedRolled = ResolveNumber(rolledNumber);

            // Glass chip: all non-zero rolls count as the chosen number
            var glass = inv.GetChip(ChipId.Glass);
            if (glass != null && glass.TargetNumbers != null && glass.TargetNumbers.Length > 0)
            {
                int glassNum = glass.TargetNumbers[0];
                if (betNumber == glassNum && resolvedRolled != 0) return true;
            }

            // Direct match
            if (betNumber == resolvedRolled) return true;

            // OneThirtySix: rolling 1, 2, 35 or 36 also counts as 0
            if (inv.HasChip(ChipId.OneThirtySix) && betNumber == 0)
            {
                if (resolvedRolled == 1 || resolvedRolled == 2 || resolvedRolled == 35 || resolvedRolled == 36) return true;
            }

            // PlusMinus plak: tagged number also counts as hit when the roll lands within ±2.
            foreach (var p in inv.plaks)
            {
                if (p.Id != PlakId.PlusMinus || p.TargetNumbers == null || p.TargetNumbers.Length == 0) continue;
                int tag = p.TargetNumbers[0];
                if (betNumber == tag && resolvedRolled > 0 && Mathf.Abs(resolvedRolled - tag) <= 2)
                    return true;
            }

            return false;
        }

        // ==============================================================
        //  Spin result processing (before player confirms)
        // ==============================================================

        /// <summary>
        /// Process the spin result for board-triggered effects.
        /// These trigger regardless of whether the player bet on the number.
        /// Call after each spin (including rerolls).
        /// </summary>
        public SpinEffects ProcessSpinResult(int rolledNumber, int currentQuota)
        {
            var fx = new SpinEffects { ChipsToDestroy = new List<ChipInstance>() };
            var color = GetNumberColor(rolledNumber);

            // Gray: instant loss
            if (color == NumberColor.Gray)
                fx.InstantLoss = true;

            // White: balance = quota, return stake, then that number's white plak entry
            // reverts to default so the effect doesn't repeat.
            if (color == NumberColor.White)
            {
                fx.SetBalanceToQuota = true;
                fx.ReturnStake = true;
                foreach (var p in inv.plaks)
                {
                    if (p.Id != PlakId.White || p.TargetNumbers == null) continue;
                    bool found = false;
                    for (int i = 0; i < p.TargetNumbers.Length; i++)
                        if (p.TargetNumbers[i] == rolledNumber) { found = true; break; }
                    if (!found) continue;
                    if (p.TriggeredNumbers == null) p.TriggeredNumbers = new List<int>();
                    if (!p.TriggeredNumbers.Contains(rolledNumber))
                        p.TriggeredNumbers.Add(rolledNumber);
                }
            }

            // Yellow: +2 gold chips (triggers on hit regardless of bet)
            if (color == NumberColor.Yellow)
                fx.GoldChipsGained += 2;

            // Orange: +1 reroll this quota
            if (color == NumberColor.Orange)
                fx.RerollsAdded++;

            // Blue: free reroll on first hit per bet
            if (color == NumberColor.Blue && TryTrigger("blue_" + rolledNumber))
                fx.AutoReroll = true;

            // Auto-reroll chips (fire on first occurrence per bet)
            if (rolledNumber > 0)
            {
                var stdColor = RouletteData.ColorOf(rolledNumber);

                // AntiRed
                if (stdColor == BetColor.Red && inv.HasChip(ChipId.AntiRed) && TryTrigger("antired"))
                    fx.AutoReroll = true;

                // AntiBlack
                if (stdColor == BetColor.Black && inv.HasChip(ChipId.AntiBlack) && TryTrigger("antiblack"))
                    fx.AutoReroll = true;

                // CrossedTwelve
                int dozen = RouletteData.DozenOf(rolledNumber);
                foreach (var c in inv.equippedChips)
                {
                    if (c.Destroyed || c.Id != ChipId.CrossedTwelve) continue;
                    if (c.TargetGroup == dozen && TryTrigger("crossed12_" + c.TargetGroup))
                        fx.AutoReroll = true;
                }

            }

            // Glass chip: any non-zero roll counts as the chosen number, so the chip is consumed.
            // Zero rolls are excluded from Glass by design.
            var glassChip = inv.GetChip(ChipId.Glass);
            if (glassChip != null && glassChip.TargetNumbers != null && glassChip.TargetNumbers.Length > 0 && rolledNumber != 0)
            {
                fx.ChipsToDestroy.Add(glassChip);
            }

            return fx;
        }

        // ==============================================================
        //  Payout computation (when bet wins)
        // ==============================================================

        /// <summary>
        /// Computes the effective multiplier for a given category,
        /// including all chip/card/ball bonuses.
        /// Only call when the bet actually wins.
        /// </summary>
        public int ComputeMultiplier(
            BetCategory category,
            int rolledNumber,
            Dictionary<int, int> currentBet,
            int goldChipCount,
            List<BallRollResult> scoringBalls = null)
        {
            int baseMult = RouletteData.MultiplierFor(category);

            // Effective rolled number after board replacements (Spikes).
            // All number-property chip checks below use `effective` so replacement chips truly
            // inherit the chosen number's color, values, and modifiers.
            int effective = ResolveNumber(rolledNumber);

            // Purple plak: +5 to number category multiplier. Check the recolor at the rolled
            // position first; fall back to the effective number so a Spikes'd position inherits
            // a Purple chosen number.
            var purpleColor = GetNumberColor(rolledNumber);
            if (purpleColor == NumberColor.Default && effective != rolledNumber)
                purpleColor = GetNumberColor(effective);
            if (category == BetCategory.Number && purpleColor == NumberColor.Purple)
                baseMult += 5;

            // Card bonuses (permanent category mult increases)
            baseMult += GetCategoryBonus(category);

            // Space: +2 to Number, -1 to every other base category.
            if (inv.HasChip(ChipId.Space))
                baseMult += (category == BetCategory.Number) ? 2 : -1;

            // Chip additive + multiplicative bonuses
            int additive = 0;

            // Gray plaks grant a permanent +3 mult each.
            foreach (var p in inv.plaks)
                if (p.Id == PlakId.Gray) additive += 3;
            int multiplicative = 1;
            float multiplicativeFloat = 1f;

            bool allBallsRed   = AllScoringBallsMatchColor(scoringBalls, BetColor.Red);
            bool allBallsBlack = AllScoringBallsMatchColor(scoringBalls, BetColor.Black);

            foreach (var c in inv.equippedChips)
            {
                if (c.Destroyed) continue;

                switch (c.Id)
                {
                    case ChipId.Bordeaux:
                        if (effective > 0 && RouletteData.ColorOf(effective) == BetColor.Red)
                            additive += 1;
                        break;

                    case ChipId.Dark:
                        if (effective > 0 && RouletteData.ColorOf(effective) == BetColor.Black)
                            additive += 1;
                        break;

                    case ChipId.BlackDozen:
                        if (effective > 0 && RouletteData.DozenOf(effective) == c.TargetGroup)
                            additive += 2;
                        break;

                    case ChipId.Seven:
                        if (c.TargetNumbers != null)
                            foreach (int tn in c.TargetNumbers)
                                if (tn == effective) { additive += 4; break; }
                        break;

                    case ChipId.Six:
                        if (c.TargetNumbers != null)
                            foreach (int tn in c.TargetNumbers)
                                if (tn == effective) { additive += 3; break; }
                        break;

                    case ChipId.Zero:
                        if (effective == 0)
                            additive += 10;
                        break;

                    case ChipId.Even:
                        if (effective > 0 && effective % 2 == 0)
                            additive += 1;
                        break;

                    case ChipId.Odd:
                        if (effective > 0 && effective % 2 == 1)
                            additive += 1;
                        break;

                    case ChipId.Three:
                        if (effective > 0 && effective % 3 == 0)
                            additive += 2;
                        break;

                    case ChipId.Prime:
                        if (Primes.Contains(effective))
                            additive += 2;
                        break;

                    case ChipId.High:
                        if (effective > 18)
                            additive += 1;
                        break;

                    case ChipId.Low:
                        if (effective > 0 && effective < 18)
                            additive += 1;
                        break;

                    case ChipId.Clover:
                        additive += 2;
                        break;

                    case ChipId.Coin:
                        additive += 2;
                        break;

                    case ChipId.GoldBar:
                        additive += goldChipCount;
                        break;

                    case ChipId.Dots:
                        additive += Mathf.Max(0, inv.ActiveChipCount - 1); // other chips
                        break;

                    case ChipId.X:
                        additive += 1;
                        break;

                    case ChipId.BallChip:
                        additive += inv.ActiveBallCount;
                        break;

                    case ChipId.AntiBall:
                        additive += 3;
                        break;

                    case ChipId.D6:
                        additive += 2;
                        break;

                    case ChipId.RedBank:
                    case ChipId.BlackBank:
                    case ChipId.PrimeBank:
                    case ChipId.GreenBank:
                        additive += c.AccumulatedBonus;
                        break;

                    case ChipId.AntiRoll:
                        additive += inv.unusedRerollsRun;
                        break;

                    case ChipId.Holey:
                        int emptySlots = Mathf.Max(0, GetChipSlots() - inv.ActiveChipCount);
                        additive += 2 * emptySlots;
                        break;

                    case ChipId.Loss:
                        multiplicative *= 2;
                        break;

                    case ChipId.VaBank:
                        // x3 mult only if the single bet is on 0
                        if (currentBet != null && currentBet.Count == 1 && currentBet.ContainsKey(0))
                            multiplicative *= 3;
                        break;

                    case ChipId.Luck4:
                        if (effective > 0 && effective % 4 == 0)
                            multiplicative *= 2;
                        break;

                    case ChipId.Luck6:
                        if (effective > 0 && effective % 6 == 0)
                            multiplicative *= 3;
                        break;

                    case ChipId.Nine:
                        if (NineNumbers.Contains(effective))
                            multiplicative *= 2;
                        break;

                    case ChipId.LastChance:
                        if (CurrentRerollsRemaining <= 0)
                            multiplicative *= 3;
                        break;

                    case ChipId.EncoreMore:
                        // On the last remaining bet of the quota (the one being resolved now).
                        if (CurrentBetsRemaining <= 1)
                            multiplicative *= 2;
                        break;

                    // ---- Float multipliers ----
                    case ChipId.Luck3:
                        if (effective > 0 && effective % 3 == 0)
                            multiplicativeFloat *= 1.5f;
                        break;

                    case ChipId.XRed:
                        if (allBallsRed)
                            multiplicativeFloat *= 1.5f;
                        break;

                    case ChipId.XBlack:
                        if (allBallsBlack)
                            multiplicativeFloat *= 1.5f;
                        break;

                    case ChipId.Transparent:
                    {
                        int empty = Mathf.Max(0, GetChipSlots() - inv.ActiveChipCount);
                        multiplicativeFloat *= (1f + 0.5f * empty);
                        break;
                    }

                    case ChipId.Champion:
                        multiplicativeFloat *= (1f + 0.25f * inv.quotasBeatenDouble);
                        break;

                    case ChipId.LuckyNumber:
                        multiplicativeFloat *= (1f + 0.2f * inv.wonNumberBets);
                        break;
                }
            }

            // Ball bonuses (from scoring balls)
            if (scoringBalls != null)
            {
                foreach (var br in scoringBalls)
                {
                    additive += br.AdditiveBonus;
                    multiplicative *= br.MultiplyBy;
                }
            }

            float finalFloat = (baseMult + additive) * multiplicative * multiplicativeFloat;
            int finalMult = Mathf.RoundToInt(finalFloat);
            return Mathf.Max(0, finalMult);
        }

        static bool AllScoringBallsMatchColor(List<BallRollResult> balls, BetColor color)
        {
            if (balls == null || balls.Count == 0) return false;
            int matched = 0;
            foreach (var br in balls)
            {
                if (!br.Scores) continue;
                if (br.RolledNumber <= 0) return false;
                if (RouletteData.ColorOf(br.RolledNumber) != color) return false;
                matched++;
            }
            return matched > 0;
        }

        /// <summary>
        /// Full payout calculation across all bets placed.
        /// Each scoring ball contributes its own independent payout based on its rolled number.
        /// Returns total payout and the best winning category.
        /// </summary>
        public int ComputeTotalPayout(
            int rolledNumber,
            Dictionary<int, int> currentBet,
            int goldChipCount,
            List<BallRollResult> scoringBalls,
            out BetCategory bestCategory,
            out bool anyWin)
        {
            int totalPayout = 0;
            bestCategory = BetCategory.Color;
            anyWin = false;

            // Build list of per-ball rolls. Each scoring ball contributes independently.
            // Fallback to single rolledNumber if there are no balls.
            var rolls = new List<BallRollResult>();
            if (scoringBalls != null && scoringBalls.Count > 0)
            {
                rolls.AddRange(scoringBalls);
            }
            else
            {
                rolls.Add(new BallRollResult { RolledNumber = rolledNumber, MultiplyBy = 1 });
            }

            int bestEffective = -1;
            foreach (var ballRoll in rolls)
            {
                int rolled = ballRoll.RolledNumber;
                // Each ball's additive/multiplicative applies only to its own contribution.
                var singleBall = new List<BallRollResult> { ballRoll };

                foreach (var kvp in currentBet)
                {
                    int betNum = kvp.Key;
                    int amount = kvp.Value;

                    BetCategory? best = BestCategoryFor(betNum, rolled);
                    if (best.HasValue)
                    {
                        int mult = ComputeMultiplier(best.Value, rolled, currentBet, goldChipCount, singleBall);
                        totalPayout += amount * mult;
                        if (!anyWin || mult > bestEffective)
                        {
                            bestCategory = best.Value;
                            bestEffective = mult;
                        }
                        anyWin = true;
                    }
                }
            }

            return totalPayout;
        }

        /// <summary>
        /// Returns the best matching category for a bet on betNum when rolledNumber lands.
        /// Uses item-aware hit detection (MoreLess, Glass, OneThirtySix, Spikes, etc.)
        /// </summary>
        public BetCategory? BestCategoryFor(int betNum, int rolled)
        {
            int resolvedRolled = ResolveNumber(rolled);

            // Check if glass chip makes everything one number (Glass does not trigger on 0 rolls).
            var glass = inv.GetChip(ChipId.Glass);
            if (glass != null && glass.TargetNumbers != null && glass.TargetNumbers.Length > 0 && resolvedRolled != 0)
            {
                resolvedRolled = glass.TargetNumbers[0];
            }

            // Bet on 0 only covers Zero
            if (betNum == 0)
            {
                bool zeroHit = resolvedRolled == 0;
                // OneThirtySix: 1, 2, 35 and 36 also count as 0
                if (!zeroHit && inv.HasChip(ChipId.OneThirtySix)
                    && (rolled == 1 || rolled == 2 || rolled == 35 || rolled == 36))
                    zeroHit = true;
                return zeroHit ? (BetCategory?)BetCategory.Zero : null;
            }

            BetCategory? best = null;
            int bestMult = -1;

            void Try(BetCategory cat, bool match)
            {
                if (!match) return;
                // Use effective mult (base + card bonus) so a bonus that makes Color beat Dozen
                // actually gets picked. Chip-additives apply equally to any category so they
                // don't affect ordering.
                int m = RouletteData.MultiplierFor(cat) + GetCategoryBonus(cat);
                if (m > bestMult) { bestMult = m; best = cat; }
            }

            // Number match (with MoreLess expansion)
            bool numberHit = IsNumberHit(betNum, rolled);
            Try(BetCategory.Number, numberHit);

            // Category matches (only for non-zero rolled)
            if (resolvedRolled != 0)
            {
                Try(BetCategory.Dozen, RouletteData.DozenOf(betNum) == RouletteData.DozenOf(resolvedRolled));

                // Color match: use board color for the rolled number
                var rolledBoardColor = GetNumberColor(rolled);
                BetColor rolledColor;
                if (rolledBoardColor == NumberColor.Red)
                    rolledColor = BetColor.Red;
                else if (rolledBoardColor == NumberColor.Black)
                    rolledColor = BetColor.Black;
                else if (rolledBoardColor == NumberColor.Default)
                    rolledColor = RouletteData.ColorOf(resolvedRolled);
                else
                    rolledColor = BetColor.None; // non-standard recolors (Purple, Gray, etc.) remove from color matching

                BetColor betColor = RouletteData.ColorOf(betNum);
                Try(BetCategory.Color, betColor == rolledColor && rolledColor != BetColor.None);
            }

            return best;
        }

        // ==============================================================
        //  Ball system
        // ==============================================================

        /// <summary>
        /// Rolls all balls, determines which score, and computes ball-specific bonuses.
        /// </summary>
        public List<BallRollResult> RollBalls(int chosenBallIndex = 0)
        {
            var results = new List<BallRollResult>();
            var activeBalls = new List<BallInstance>();

            foreach (var b in inv.ownedBalls)
                if (!b.Destroyed) activeBalls.Add(b);

            if (activeBalls.Count == 0)
                return results;

            // Roll each ball
            for (int i = 0; i < activeBalls.Count; i++)
            {
                var ball = activeBalls[i];
                int rolled = RollForBall(ball);

                var result = new BallRollResult
                {
                    Ball = ball,
                    RolledNumber = rolled,
                    Scores = false,
                    AdditiveBonus = 0,
                    MultiplyBy = 1,
                    ShouldDestroy = false
                };

                results.Add(result);
            }

            // Determine which balls score
            bool randomChoice = IsRandomBallChoice();
            int choices = GetBallChoices();

            if (randomChoice)
            {
                // D6 chip: random selection
                var indices = new List<int>();
                for (int i = 0; i < results.Count; i++) indices.Add(i);
                for (int c = 0; c < Mathf.Min(choices, results.Count); c++)
                {
                    int pick = rng.Next(indices.Count);
                    results[indices[pick]] = SetScoring(results[indices[pick]], true);
                    indices.RemoveAt(pick);
                }
            }
            else
            {
                // Player choice (default: first N balls, or specified index)
                for (int c = 0; c < Mathf.Min(choices, results.Count); c++)
                {
                    int idx = (chosenBallIndex + c) % results.Count;
                    results[idx] = SetScoring(results[idx], true);
                }
            }

            // Stone ball always scores
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].Ball.Id == BallId.Stone)
                    results[i] = SetScoring(results[i], true);
            }

            // Apply ball-specific effects to ALL balls so their bonuses survive
            // the player switching their chosen ball via SelectBall().
            // Destruction is gated on `Scores` at confirm time (see RouletteGame.ConfirmResult).
            for (int i = 0; i < results.Count; i++)
            {
                results[i] = ApplyBallEffect(results[i]);
            }

            return results;
        }

        int RollForBall(BallInstance ball)
        {
            int rolled = RollFromPool();

            // Magnet balls reroll until they land on their target color
            if (ball.Id == BallId.RedMagnet)
            {
                int safety = 100;
                while (rolled == 0 || RouletteData.ColorOf(rolled) != BetColor.Red)
                {
                    rolled = RollFromPool();
                    if (--safety <= 0) break;
                }
            }
            else if (ball.Id == BallId.BlackMagnet)
            {
                int safety = 100;
                while (rolled == 0 || RouletteData.ColorOf(rolled) != BetColor.Black)
                {
                    rolled = RollFromPool();
                    if (--safety <= 0) break;
                }
            }

            return rolled;
        }

        /// <summary>
        /// Roll a number 0..36, skipping any that have been removed by Boom / BigBang.
        /// Falls back to a plain roll if everything would be excluded.
        /// </summary>
        public int RollFromPool()
        {
            // Build the weighted pool: each non-removed number gets 1 entry,
            // plus 2 extra entries per Plus plak targeting that number.
            var pool = new List<int>(40);
            for (int n = 0; n <= 36; n++)
                if (!IsNumberRemoved(n)) pool.Add(n);

            foreach (var p in inv.plaks)
            {
                if (p.Id != PlakId.Plus || p.TargetNumbers == null || p.TargetNumbers.Length == 0) continue;
                int tgt = p.TargetNumbers[0];
                if (IsNumberRemoved(tgt)) continue;
                pool.Add(tgt);
                pool.Add(tgt);
            }

            if (pool.Count == 0) return rng.Next(0, 37);
            return pool[rng.Next(pool.Count)];
        }

        BallRollResult ApplyBallEffect(BallRollResult r)
        {
            switch (r.Ball.Id)
            {
                case BallId.Glass:
                    r.MultiplyBy *= 2;
                    if (rng.NextDouble() < 0.5) r.ShouldDestroy = true;
                    break;

                case BallId.Bouncy:
                    r.AdditiveBonus += rng.Next(1, 6); // 1 to 5
                    break;

                case BallId.Red:
                    if (r.RolledNumber > 0 && RouletteData.ColorOf(r.RolledNumber) == BetColor.Red)
                        r.AdditiveBonus += 5;
                    break;

                case BallId.Black:
                    if (r.RolledNumber > 0 && RouletteData.ColorOf(r.RolledNumber) == BetColor.Black)
                        r.AdditiveBonus += 5;
                    break;

                case BallId.Green:
                    if (r.RolledNumber == 0)
                        r.MultiplyBy *= 2;
                    break;

                case BallId.Explosive:
                    r.AdditiveBonus += rng.Next(1, 11); // 1 to 10
                    if (rng.NextDouble() < 0.15) r.ShouldDestroy = true;
                    break;

                case BallId.RedMagnet:
                case BallId.BlackMagnet:
                    // Magnet effect is in RollForBall; destruction tracked via BetsUsed
                    break;
            }

            return r;
        }

        static BallRollResult SetScoring(BallRollResult r, bool scores)
        {
            r.Scores = scores;
            return r;
        }

        // ==============================================================
        //  Loss prevention
        // ==============================================================

        /// <summary>
        /// Checks if the Skull chip should save the player from game over.
        /// If true, the chip is marked as destroyed.
        /// </summary>
        public bool TrySkullSave()
        {
            var skull = inv.GetChip(ChipId.Skull);
            if (skull != null)
            {
                skull.Destroyed = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// If Banana food is active and balance just hit 0 (first time this quota),
        /// returns 200 money to give the player.
        /// </summary>
        public int CheckBanana(int currentMoney)
        {
            if (inv.hasBanana && !inv.bananaUsedThisQuota && currentMoney <= 0)
            {
                inv.bananaUsedThisQuota = true;
                return 200;
            }
            return 0;
        }

        /// <summary>
        /// If Grape food is active, returns 50% of the stake to refund on a lost bet.
        /// </summary>
        public int GetGrapeRefund(int totalStake)
        {
            if (inv.hasGrape)
                return totalStake / 2;
            return 0;
        }

        // ==============================================================
        //  Helpers
        // ==============================================================

        public int GetCategoryBonus(BetCategory cat)
        {
            switch (cat)
            {
                case BetCategory.Color:  return inv.bonusColorMult;
                case BetCategory.Dozen:  return inv.bonusDozenMult;
                case BetCategory.Number: return inv.bonusNumberMult;
                case BetCategory.Zero:   return inv.bonusZeroMult;
                default: return 0;
            }
        }

        bool TryTrigger(string key)
        {
            if (firedTriggers.Contains(key)) return false;
            firedTriggers.Add(key);
            return true;
        }

    }
}
