using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeadlyRoulette
{
    public enum ShopItemKind { Chip, Card, Food, BallTicket }

    public enum ChipSelectionMode { None, Numbers, Dozen }

    [Serializable]
    public class ShopSlot
    {
        public bool empty;
        public ShopItemKind kind;
        public int cost;
        public ChipId chipId;
        public CardId cardId;
        public FoodId foodId;
        public Rarity rarity;

        public string DisplayName
        {
            get
            {
                if (empty) return "";
                switch (kind)
                {
                    case ShopItemKind.Chip:          return ItemRegistry.Chips[chipId].Name;
                    case ShopItemKind.Card:          return ItemRegistry.Cards[cardId].Name;
                    case ShopItemKind.Food:          return ItemRegistry.Foods[foodId].Name;
                    case ShopItemKind.BallTicket:    return "Ball Ticket";
                    default: return "";
                }
            }
        }

        public string Description
        {
            get
            {
                if (empty) return "";
                switch (kind)
                {
                    case ShopItemKind.Chip:          return ItemRegistry.Chips[chipId].Description;
                    case ShopItemKind.Card:          return ItemRegistry.Cards[cardId].Description;
                    case ShopItemKind.Food:          return ItemRegistry.Foods[foodId].Description;
                    case ShopItemKind.BallTicket:    return "Get a random ball.";
                    default: return "";
                }
            }
        }

        public string RarityLabel
        {
            get
            {
                if (empty) return "";
                switch (kind)
                {
                    case ShopItemKind.Chip:
                    case ShopItemKind.Card:
                    case ShopItemKind.Food:
                        return rarity.ToString();
                    case ShopItemKind.BallTicket:    return "Common";
                    default: return "";
                }
            }
        }
    }

    /// <summary>
    /// Manages the shop: generating inventory, buying, selling, boosters, ball tickets.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] RouletteGame game;

        public const int RandomPoolSize = 5;
        public const int FoodRefreshQuotas = 3;
        public const int OtherRefreshQuotas = 3;
        public const int BallTicketCost = 5;
        public const int BaseMoneyUpgradeBaseCost = 1;
        public const int BaseMoneyUpgradeAmount = 100;

        // Each purchase of the base-money upgrade raises its price by 1 for the rest of the run.
        public int baseMoneyUpgradesPurchased;
        public int CurrentBaseMoneyUpgradeCost => BaseMoneyUpgradeBaseCost + baseMoneyUpgradesPurchased;

        // Shop inventory
        public ShopSlot[] randomPool = new ShopSlot[RandomPoolSize];
        public ShopSlot foodSlot;   // null until first GenerateShop — avoids stale default kind
        public ShopSlot[] otherSlots = new ShopSlot[2];

        int quotasSinceFoodRefresh;
        int quotasSinceOtherRefresh;
        int rerollCount;

        System.Random rng = new System.Random();

        // Cached item lists by rarity
        static ChipId[] commonChips, rareChips, epicChips;
        static CardId[] commonCards, rareCards;
        static FoodId[] allFoods;
        static BallId[] allBalls;

        public event Action OnShopChanged;

        void Awake()
        {
            CacheItemLists();
            if (game == null) game = GetComponent<RouletteGame>();
        }

        static void CacheItemLists()
        {
            if (commonChips != null) return;
            commonChips = ItemRegistry.Chips.Where(kv => kv.Value.Rarity == Rarity.Common).Select(kv => kv.Key).ToArray();
            rareChips   = ItemRegistry.Chips.Where(kv => kv.Value.Rarity == Rarity.Rare).Select(kv => kv.Key).ToArray();
            epicChips   = ItemRegistry.Chips.Where(kv => kv.Value.Rarity == Rarity.Epic).Select(kv => kv.Key).ToArray();
            commonCards = ItemRegistry.Cards.Where(kv => kv.Value.Rarity == Rarity.Common).Select(kv => kv.Key).ToArray();
            rareCards   = ItemRegistry.Cards.Where(kv => kv.Value.Rarity == Rarity.Rare).Select(kv => kv.Key).ToArray();
            allFoods    = ItemRegistry.Foods.Keys.ToArray();
            allBalls    = ItemRegistry.Balls.Keys.ToArray();
        }

        // ==============================================================
        //  Shop generation
        // ==============================================================

        /// <summary>
        /// Generate fresh shop contents. Call between quotas.
        /// </summary>
        public void GenerateShop()
        {
            CacheItemLists();

            if (randomPool == null || randomPool.Length != RandomPoolSize)
                randomPool = new ShopSlot[RandomPoolSize];

            // Random pool: full regenerate each quota
            for (int i = 0; i < randomPool.Length; i++)
                randomPool[i] = GenerateRandomPoolSlot();

            // Reroll cost resets each quota.
            rerollCount = 0;

            // Food: refresh every FoodRefreshQuotas (or immediately if never generated / kind got corrupted).
            // Once purchased (slot.empty) it stays empty until the next refresh cycle.
            quotasSinceFoodRefresh++;
            bool needFood = foodSlot == null || foodSlot.kind != ShopItemKind.Food || quotasSinceFoodRefresh >= FoodRefreshQuotas;
            if (needFood)
            {
                foodSlot = GenerateFoodSlot();
                quotasSinceFoodRefresh = 0;
            }

            // Other items (ball tickets): refresh every OtherRefreshQuotas.
            // Empty (purchased) slots persist until refresh.
            quotasSinceOtherRefresh++;
            bool needOther = otherSlots == null || otherSlots.Length != 2
                             || otherSlots[0] == null || otherSlots[0].kind != ShopItemKind.BallTicket
                             || quotasSinceOtherRefresh >= OtherRefreshQuotas;
            if (needOther)
            {
                if (otherSlots == null || otherSlots.Length != 2) otherSlots = new ShopSlot[2];
                for (int i = 0; i < otherSlots.Length; i++)
                    otherSlots[i] = GenerateOtherSlot();
                quotasSinceOtherRefresh = 0;
            }

            OnShopChanged?.Invoke();
        }

        ShopSlot GenerateRandomPoolSlot()
        {
            // Random pool is chips only. Weighted: common 50%, rare 35%, epic 15%.
            int roll = rng.Next(100);
            var slot = new ShopSlot { kind = ShopItemKind.Chip };

            if (roll < 50)
            {
                slot.chipId = commonChips[rng.Next(commonChips.Length)];
                slot.rarity = Rarity.Common;
                slot.cost = 4;
            }
            else if (roll < 85)
            {
                slot.chipId = rareChips[rng.Next(rareChips.Length)];
                slot.rarity = Rarity.Rare;
                slot.cost = 6;
            }
            else
            {
                slot.chipId = epicChips[rng.Next(epicChips.Length)];
                slot.rarity = Rarity.Epic;
                slot.cost = 10;
            }

            return slot;
        }

        ShopSlot GenerateFoodSlot()
        {
            return new ShopSlot
            {
                kind = ShopItemKind.Food,
                foodId = allFoods[rng.Next(allFoods.Length)],
                rarity = Rarity.Rare,
                cost = 10
            };
        }

        ShopSlot GenerateOtherSlot()
        {
            return new ShopSlot
            {
                kind = ShopItemKind.BallTicket,
                cost = BallTicketCost
            };
        }

        // ==============================================================
        //  Reroll
        // ==============================================================

        /// <summary>Cost of the next reroll. Starts at 2, grows by 1 per reroll, resets each quota.</summary>
        public int RerollCost => 2 + rerollCount;

        public bool CanReroll()
        {
            return game.goldChips >= RerollCost;
        }

        /// <summary>
        /// Reroll the random pool only. Other sections (food, ball tickets, base money upgrade)
        /// are not affected. Cost scales per reroll within a quota.
        /// </summary>
        public void RerollShop()
        {
            if (!CanReroll()) return;
            game.goldChips -= RerollCost;
            rerollCount++;

            CacheItemLists();
            for (int i = 0; i < randomPool.Length; i++)
                if (randomPool[i] != null && !randomPool[i].empty)
                    randomPool[i] = GenerateRandomPoolSlot();

            OnShopChanged?.Invoke();
        }

        // ==============================================================
        //  Buying
        // ==============================================================

        public bool CanAfford(int cost)
        {
            return game.goldChips >= cost;
        }

        public bool CanBuyChip(PlayerInventory inv, EffectProcessor fx)
        {
            return inv.ActiveChipCount < fx.GetChipSlots();
        }

        public bool CanBuyBall(PlayerInventory inv, EffectProcessor fx)
        {
            return inv.ActiveBallCount < fx.GetBallSlots();
        }

        /// <summary>
        /// Buy a chip from a slot. Returns the new ChipInstance (needs number selection if applicable).
        /// </summary>
        public ChipInstance BuyChip(ShopSlot slot, PlayerInventory inv, int[] chosenNumbers, int chosenGroup)
        {
            if (slot.empty || slot.kind != ShopItemKind.Chip) return null;
            if (!CanAfford(slot.cost)) return null;

            game.goldChips -= slot.cost;

            var chip = new ChipInstance { Id = slot.chipId };

            // Assign chosen numbers
            if (chosenNumbers != null && chosenNumbers.Length > 0)
                chip.TargetNumbers = chosenNumbers;

            // Assign chosen group
            chip.TargetGroup = chosenGroup;

            // Spikes: player chooses target number, 3 random board positions are replaced with it.
            // Index [0] is chosen, [1..] are positions.
            if (slot.chipId == ChipId.Spikes)
                chip.TargetNumbers = BuildReplacementTargets(ResolveChosen(chosenNumbers), 3);
            else if (slot.chipId == ChipId.Six)
                chip.TargetNumbers = PickConsecutiveNumbers(6);
            else
            {
                // Assign random numbers where needed
                int randomCount = GetRandomAssignCount(slot.chipId);
                if (randomCount > 0)
                {
                    var randomNums = PickRandomNumbers(randomCount, chosenNumbers);
                    if (chip.TargetNumbers != null)
                    {
                        var combined = new int[chip.TargetNumbers.Length + randomNums.Length];
                        chip.TargetNumbers.CopyTo(combined, 0);
                        randomNums.CopyTo(combined, chip.TargetNumbers.Length);
                        chip.TargetNumbers = combined;
                    }
                    else
                    {
                        chip.TargetNumbers = randomNums;
                    }
                }
            }

            // Random dozen assignment for chips that need it
            if (slot.chipId == ChipId.BlackDozen || slot.chipId == ChipId.CrossedTwelve)
                chip.TargetGroup = rng.Next(1, 4);

            inv.equippedChips.Add(chip);
            slot.empty = true;
            OnShopChanged?.Invoke();
            return chip;
        }

        /// <summary>Buy and immediately apply a card.</summary>
        public void BuyCard(ShopSlot slot, PlayerInventory inv)
        {
            if (slot.empty || slot.kind != ShopItemKind.Card) return;
            if (!CanAfford(slot.cost)) return;

            game.goldChips -= slot.cost;
            inv.ConsumeCard(slot.cardId);
            slot.empty = true;
            OnShopChanged?.Invoke();
        }

        /// <summary>Buy and immediately apply food.</summary>
        public void BuyFood(PlayerInventory inv)
        {
            if (foodSlot == null || foodSlot.empty || foodSlot.kind != ShopItemKind.Food) return;
            if (!CanAfford(foodSlot.cost)) return;

            game.goldChips -= foodSlot.cost;
            inv.ConsumeFood(foodSlot.foodId);
            foodSlot.empty = true;
            OnShopChanged?.Invoke();
        }

        /// <summary>Roll a random multiplier-upgrade card (70% common, 30% rare).
        /// Used by the post-boss upgrade screen.</summary>
        public CardId RollRandomCard()
        {
            CacheItemLists();
            if (rng.Next(100) < 70)
                return commonCards[rng.Next(commonCards.Length)];
            else
                return rareCards[rng.Next(rareCards.Length)];
        }

        /// <summary>Always-available permanent upgrade: +100 base money for the rest of the run.
        /// Costs 1 gold, repeatable while affordable.</summary>
        public bool BuyBaseMoneyUpgrade(PlayerInventory inv)
        {
            int cost = CurrentBaseMoneyUpgradeCost;
            if (!CanAfford(cost)) return false;
            game.goldChips -= cost;
            inv.bonusBaseMoney += BaseMoneyUpgradeAmount;
            baseMoneyUpgradesPurchased++;
            OnShopChanged?.Invoke();
            return true;
        }

        /// <summary>Buy a ball ticket: returns a random BallInstance.</summary>
        public BallInstance BuyBallTicket(ShopSlot slot)
        {
            if (slot.empty || slot.kind != ShopItemKind.BallTicket) return null;
            if (!CanAfford(slot.cost)) return null;

            game.goldChips -= slot.cost;
            slot.empty = true;

            CacheItemLists();
            var ballId = allBalls[rng.Next(allBalls.Length)];
            var ball = new BallInstance { Id = ballId };

            OnShopChanged?.Invoke();
            return ball;
        }

        /// <summary>Accept a rolled ball into inventory.</summary>
        public void AcceptBall(PlayerInventory inv, BallInstance ball)
        {
            if (ball == null) return;
            inv.ownedBalls.Add(ball);
            OnShopChanged?.Invoke();
        }

        /// <summary>Sell a rolled ball for 3 gold (not added to inventory).</summary>
        public void SellRolledBall()
        {
            game.goldChips += 3;
            OnShopChanged?.Invoke();
        }

        // ==============================================================
        //  Selling from inventory
        // ==============================================================

        public int GetChipSellPrice(ChipId chipId)
        {
            int buyCost = 4;
            if (ItemRegistry.Chips.TryGetValue(chipId, out var def))
            {
                switch (def.Rarity)
                {
                    case Rarity.Common: buyCost = 4; break;
                    case Rarity.Rare:   buyCost = 6; break;
                    case Rarity.Epic:   buyCost = 10; break;
                }
            }
            return Mathf.Max(1, buyCost / 4);
        }

        public void SellChip(PlayerInventory inv, ChipInstance chip)
        {
            if (chip == null || !inv.equippedChips.Contains(chip)) return;
            int price = GetChipSellPrice(chip.Id);
            game.goldChips += price;
            inv.equippedChips.Remove(chip);
            OnShopChanged?.Invoke();
        }

        public bool CanSellBall(PlayerInventory inv)
        {
            return inv.ActiveBallCount > 1;
        }

        public void SellBall(PlayerInventory inv, BallInstance ball)
        {
            if (ball == null || !inv.ownedBalls.Contains(ball)) return;
            if (!CanSellBall(inv)) return;
            game.goldChips += 3;
            inv.ownedBalls.Remove(ball);
            OnShopChanged?.Invoke();
        }

        // ==============================================================
        //  Chip selection helpers
        // ==============================================================

        public static ChipSelectionMode GetSelectionMode(ChipId id)
        {
            switch (id)
            {
                case ChipId.Glass:
                case ChipId.Spikes:
                    return ChipSelectionMode.Numbers;

                default:
                    return ChipSelectionMode.None;
            }
        }

        public static int GetNumberChoiceCount(ChipId id)
        {
            switch (id)
            {
                case ChipId.Glass:
                case ChipId.Spikes:
                    return 1;
                default:
                    return 0;
            }
        }

        public static bool CanSelectZero(ChipId id)
        {
            if (id == ChipId.Glass) return false;
            if (id == ChipId.Spikes) return false;
            return true;
        }

        static int GetRandomAssignCount(ChipId id)
        {
            switch (id)
            {
                case ChipId.Seven:
                    return 4;
                default:
                    return 0;
            }
        }

        bool IsNumberRemoved(int n)
        {
            var fx = game != null ? game.Fx : null;
            return fx != null && fx.IsNumberRemoved(n);
        }

        int[] PickRandomNumbersFromColor(int count, BetColor color)
        {
            var available = new List<int>();
            for (int i = 1; i <= 36; i++)
                if (RouletteData.ColorOf(i) == color && !IsNumberRemoved(i)) available.Add(i);

            var picked = new int[count];
            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int idx = rng.Next(available.Count);
                picked[i] = available[idx];
                available.RemoveAt(idx);
            }
            return picked;
        }

        int[] PickRandomNumbers(int count, int[] exclude)
        {
            var available = new List<int>();
            for (int i = 1; i <= 36; i++)
            {
                if (exclude != null && Array.IndexOf(exclude, i) >= 0) continue;
                if (IsNumberRemoved(i)) continue;
                available.Add(i);
            }

            var picked = new int[count];
            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int idx = rng.Next(available.Count);
                picked[i] = available[idx];
                available.RemoveAt(idx);
            }
            return picked;
        }

        /// <summary>
        /// Pick `replaceCount` random positions (excluding the chosen number and any
        /// Boom/BigBang-removed numbers). Returns [chosen, pos1, pos2, ...].
        /// Used by Spikes, Spikes+ and Crests.
        /// </summary>
        int[] BuildReplacementTargets(int chosen, int replaceCount)
        {
            var exclude = new int[] { chosen };
            var positions = PickRandomNumbers(replaceCount, exclude);
            var result = new int[1 + positions.Length];
            result[0] = chosen;
            positions.CopyTo(result, 1);
            return result;
        }

        /// <summary>Return the player-chosen number, or a random valid board number if none was supplied.</summary>
        int ResolveChosen(int[] chosenNumbers)
        {
            if (chosenNumbers != null && chosenNumbers.Length > 0 && chosenNumbers[0] >= 1 && chosenNumbers[0] <= 36)
                return chosenNumbers[0];
            var pool = new List<int>();
            for (int i = 1; i <= 36; i++)
                if (!IsNumberRemoved(i)) pool.Add(i);
            return pool.Count > 0 ? pool[rng.Next(pool.Count)] : rng.Next(1, 37);
        }

        /// <summary>
        /// Pick `count` consecutive numbers starting from a random position (1..37-count).
        /// Retries up to a few times if the window overlaps a removed number so Six
        /// does not silently lose slots to Boom/BigBang.
        /// </summary>
        int[] PickConsecutiveNumbers(int count)
        {
            int[] best = null;
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int start = rng.Next(1, 38 - count);
                bool clean = true;
                for (int i = 0; i < count; i++)
                {
                    if (IsNumberRemoved(start + i)) { clean = false; break; }
                }
                if (clean)
                {
                    var r = new int[count];
                    for (int i = 0; i < count; i++) r[i] = start + i;
                    return r;
                }
                if (best == null)
                {
                    best = new int[count];
                    for (int i = 0; i < count; i++) best[i] = start + i;
                }
            }
            return best;
        }
    }
}
