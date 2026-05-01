using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeadlyRoulette
{
    public enum PostBossSlotKind
    {
        MultiplierCard,
        BallTicket,
        Food,
        GoldChips,
        BossChip
    }

    [Serializable]
    public class PostBossSlot
    {
        public bool claimed;
        public PostBossSlotKind kind;
        public CardId cardId;
        public FoodId foodId;
        public int goldAmount;

        public string DisplayName
        {
            get
            {
                switch (kind)
                {
                    case PostBossSlotKind.MultiplierCard: return ItemRegistry.Cards[cardId].Name;
                    case PostBossSlotKind.BallTicket:    return "Ball Ticket";
                    case PostBossSlotKind.Food:          return ItemRegistry.Foods[foodId].Name;
                    case PostBossSlotKind.GoldChips:     return goldAmount + " Gold Chips";
                    case PostBossSlotKind.BossChip:      return "Boss Chip";
                    default: return "";
                }
            }
        }

        public string Description
        {
            get
            {
                switch (kind)
                {
                    case PostBossSlotKind.MultiplierCard: return ItemRegistry.Cards[cardId].Description;
                    case PostBossSlotKind.BallTicket:    return "Get a random ball.";
                    case PostBossSlotKind.Food:          return ItemRegistry.Foods[foodId].Description;
                    case PostBossSlotKind.GoldChips:     return "+" + goldAmount + " gold.";
                    case PostBossSlotKind.BossChip:      return "+1 boss chip. Spend in the shop to modify the board.";
                    default: return "";
                }
            }
        }
    }

    /// <summary>
    /// Generates and manages the 5-slot post-boss upgrade screen.
    /// Per design: 3 slots are always a random multiplier card; 2 slots roll
    /// independently for 10% ball ticket / 30% food / 10% 5 gold chips / 40% boss chip.
    /// </summary>
    public class PostBossManager : MonoBehaviour
    {
        public const int FixedCardSlots = 3;
        public const int RandomSlots = 2;
        public const int TotalSlots = FixedCardSlots + RandomSlots;
        public const int GoldRewardAmount = 5;

        [SerializeField] RouletteGame game;
        [SerializeField] ShopManager shop;

        public PostBossSlot[] slots = new PostBossSlot[TotalSlots];

        // Pending ball waiting for the player's keep/sell decision.
        public BallInstance pendingBall;
        public int pendingBallSlotIndex = -1;

        public event Action OnChanged;

        System.Random rng = new System.Random();
        static FoodId[] allFoods;
        static BallId[] allBalls;

        void Awake()
        {
            if (game == null) game = GetComponent<RouletteGame>();
            if (shop == null) shop = GetComponent<ShopManager>();
            CacheLists();
        }

        static void CacheLists()
        {
            if (allFoods == null) allFoods = ItemRegistry.Foods.Keys.ToArray();
            if (allBalls == null) allBalls = ItemRegistry.Balls.Keys.ToArray();
        }

        public void Generate()
        {
            CacheLists();
            for (int i = 0; i < TotalSlots; i++)
            {
                if (i < FixedCardSlots)
                    slots[i] = MakeCardSlot();
                else
                    slots[i] = MakeRandomSlot();
            }
            pendingBall = null;
            pendingBallSlotIndex = -1;
            OnChanged?.Invoke();
        }

        PostBossSlot MakeCardSlot()
        {
            return new PostBossSlot
            {
                kind = PostBossSlotKind.MultiplierCard,
                cardId = shop.RollRandomCard()
            };
        }

        PostBossSlot MakeRandomSlot()
        {
            // Weights: 10% ball ticket, 30% food, 10% 5 gold, 40% boss chip.
            int roll = rng.Next(100);
            if (roll < 10)
                return new PostBossSlot { kind = PostBossSlotKind.BallTicket };
            if (roll < 40)
                return new PostBossSlot
                {
                    kind = PostBossSlotKind.Food,
                    foodId = allFoods[rng.Next(allFoods.Length)]
                };
            if (roll < 50)
                return new PostBossSlot
                {
                    kind = PostBossSlotKind.GoldChips,
                    goldAmount = GoldRewardAmount
                };
            return new PostBossSlot { kind = PostBossSlotKind.BossChip };
        }

        /// <summary>True once the player has claimed (or is resolving) any reward slot.</summary>
        public bool AnyClaimed
        {
            get
            {
                if (pendingBall != null) return true;
                foreach (var s in slots) if (s != null && s.claimed) return true;
                return false;
            }
        }

        /// <summary>Try to claim a slot. Ball tickets defer completion until the
        /// player accepts/sells the rolled ball — see <see cref="AcceptPendingBall"/>.
        /// Only one claim is allowed per post-boss screen.</summary>
        public bool Claim(int index)
        {
            if (index < 0 || index >= slots.Length) return false;
            var slot = slots[index];
            if (slot == null || slot.claimed) return false;
            // Block additional claims once any reward has been taken or is pending.
            if (AnyClaimed) return false;

            switch (slot.kind)
            {
                case PostBossSlotKind.MultiplierCard:
                    game.inventory.ConsumeCard(slot.cardId);
                    slot.claimed = true;
                    break;

                case PostBossSlotKind.Food:
                    game.inventory.ConsumeFood(slot.foodId);
                    slot.claimed = true;
                    break;

                case PostBossSlotKind.GoldChips:
                    game.goldChips += slot.goldAmount;
                    slot.claimed = true;
                    break;

                case PostBossSlotKind.BossChip:
                    game.inventory.bossChips++;
                    slot.claimed = true;
                    break;

                case PostBossSlotKind.BallTicket:
                    CacheLists();
                    pendingBall = new BallInstance { Id = allBalls[rng.Next(allBalls.Length)] };
                    pendingBallSlotIndex = index;
                    // Slot stays unclaimed until the player resolves the ball.
                    break;
            }

            OnChanged?.Invoke();
            return true;
        }

        public void AcceptPendingBall()
        {
            if (pendingBall == null) return;
            game.inventory.ownedBalls.Add(pendingBall);
            FinishPendingBall();
        }

        public void SellPendingBall()
        {
            if (pendingBall == null) return;
            game.goldChips += 3;
            FinishPendingBall();
        }

        void FinishPendingBall()
        {
            if (pendingBallSlotIndex >= 0 && pendingBallSlotIndex < slots.Length)
                slots[pendingBallSlotIndex].claimed = true;
            pendingBall = null;
            pendingBallSlotIndex = -1;
            OnChanged?.Invoke();
        }
    }
}
