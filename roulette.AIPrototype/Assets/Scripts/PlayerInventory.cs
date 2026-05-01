using System;
using System.Collections.Generic;

namespace DeadlyRoulette
{
    [Serializable]
    public class ChipInstance
    {
        public ChipId Id;

        /// <summary>Numbers chosen by the player or assigned randomly, depending on chip type.</summary>
        public int[] TargetNumbers;

        /// <summary>Chosen dozen (1-3) or six-cluster (1-6), depending on chip type.</summary>
        public int TargetGroup;

        // --- Runtime mutable state ---
        /// <summary>Accumulated bonus for bank-type chips (RedBank, BlackBank).</summary>
        public int AccumulatedBonus;

        /// <summary>Chip has been destroyed by a one-time effect.</summary>
        public bool Destroyed;
    }

    [Serializable]
    public class BallInstance
    {
        public BallId Id;

        /// <summary>Bets used so far (for magnet balls with 5-bet lifetime).</summary>
        public int BetsUsed;

        /// <summary>Ball has been destroyed.</summary>
        public bool Destroyed;
    }

    /// <summary>
    /// Holds everything the player owns: equipped chips, balls, and accumulated
    /// permanent bonuses from consumed cards and food.
    /// </summary>
    [Serializable]
    public class PlayerInventory
    {
        // ---- Slot limits (base values, modified by chips/food) ----
        public int baseChipSlots = 5;
        public int baseBallSlots = 3;
        public int baseBallChoices = 1;

        // ---- Equipped items ----
        public List<ChipInstance> equippedChips = new List<ChipInstance>();
        public List<BallInstance> ownedBalls = new List<BallInstance>();

        // ---- Permanent bonuses from Food ----
        public int bonusBetsPerQuota;       // Lemon
        public int bonusRerollsPerQuota;    // Lime
        public int bonusBaseMoney;          // Cherry
        public int bonusBaseReward;         // Apple
        public int bonusUnusedBetReward;    // Pear
        public int bonusChipSlots;          // Orange (food)
        public int bonusBallSlots;          // Strawberry
        public bool hasBanana;              // Banana: 200 money on first 0-balance per quota
        public bool hasGrape;               // Grape: 50% stake refund on loss
        public int rerollCostReduction;     // Chocolate

        // ---- Permanent bonuses from Cards (category multiplier bonuses) ----
        public int bonusColorMult;
        public int bonusDozenMult;
        public int bonusNumberMult;
        public int bonusZeroMult;

        // ---- Run-scoped counters (persist across quotas, reset on new run) ----
        public int quotasBeatenDouble;      // Champion accumulator: +1 each quota beaten at >=2x the quota target
        public int unusedRerollsRun;        // AntiRoll accumulator: total unused rerolls across the run
        public int wonNumberBets;           // LuckyNumber accumulator: won bets on a single number

        // ---- Boss chip currency (spent in shop to pick a plak) ----
        public int bossChips;

        // ---- Permanent board modifications from plaks ----
        public List<PlakInstance> plaks = new List<PlakInstance>();

        // ---- Per-quota transient state ----
        [NonSerialized] public bool bananaUsedThisQuota;
        [NonSerialized] public int betsUsedThisQuota;       // Budget chip: +50 base money per used bet

        /// <summary>Call at the start of each quota to reset transient state.</summary>
        public void ResetQuotaState()
        {
            bananaUsedThisQuota = false;
            betsUsedThisQuota = 0;
        }

        // ---- Helpers ----
        public int ActiveChipCount
        {
            get
            {
                int c = 0;
                for (int i = 0; i < equippedChips.Count; i++)
                    if (!equippedChips[i].Destroyed) c++;
                return c;
            }
        }

        public int ActiveBallCount
        {
            get
            {
                int c = 0;
                for (int i = 0; i < ownedBalls.Count; i++)
                    if (!ownedBalls[i].Destroyed) c++;
                return c;
            }
        }

        public bool HasChip(ChipId id)
        {
            for (int i = 0; i < equippedChips.Count; i++)
                if (equippedChips[i].Id == id && !equippedChips[i].Destroyed) return true;
            return false;
        }

        public ChipInstance GetChip(ChipId id)
        {
            for (int i = 0; i < equippedChips.Count; i++)
                if (equippedChips[i].Id == id && !equippedChips[i].Destroyed) return equippedChips[i];
            return null;
        }

        /// <summary>Apply a card's instant effect to permanent bonuses.</summary>
        public void ConsumeCard(CardId card)
        {
            switch (card)
            {
                case CardId.SixDiamonds:  bonusColorMult  += 1; break;
                case CardId.SixHearts:    bonusDozenMult  += 1; break;
                case CardId.SixSpades:    bonusDozenMult  += 1; break;
                case CardId.SixClubs:     bonusNumberMult += 1; break;
                case CardId.GreenSix:     bonusZeroMult   += 1; break;
                case CardId.AceDiamonds:  bonusColorMult  += 2; break;
                case CardId.AceHearts:    bonusDozenMult  += 2; break;
                case CardId.AceSpades:    bonusDozenMult  += 2; break;
                case CardId.AceClubs:     bonusNumberMult += 2; break;
                case CardId.GreenAce:     bonusZeroMult   += 2; break;
                case CardId.RedJoker:
                    bonusColorMult  += 1;
                    bonusDozenMult  += 1;
                    bonusNumberMult += 1;
                    bonusZeroMult   += 1;
                    break;
                case CardId.BlackJoker:
                    bonusColorMult  += 2;
                    bonusDozenMult  += 2;
                    bonusNumberMult += 2;
                    bonusZeroMult   += 2;
                    break;
            }
        }

        /// <summary>Apply a food's permanent effect.</summary>
        public void ConsumeFood(FoodId food)
        {
            switch (food)
            {
                case FoodId.Lemon:      bonusBetsPerQuota    += 1; break;
                case FoodId.Lime:       bonusRerollsPerQuota += 1; break;
                case FoodId.Apple:      bonusBaseReward      += 1; break;
                case FoodId.Pear:       bonusUnusedBetReward += 1; break;
                case FoodId.Cherry:     bonusBaseMoney       += 200; break;
                case FoodId.Banana:     hasBanana = true; break;
                case FoodId.Orange:     bonusChipSlots       += 1; break;
                case FoodId.Strawberry: bonusBallSlots       += 1; break;
                case FoodId.Grape:      hasGrape = true; break;
                case FoodId.Chocolate:  rerollCostReduction  += 2; break;
            }
        }
    }
}
