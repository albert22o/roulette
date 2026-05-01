using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadlyRoulette
{
    /// <summary>
    /// Rolls plak options when the player spends a boss chip and applies them
    /// as permanent modifications to the player inventory.
    /// </summary>
    public class PlakShop : MonoBehaviour
    {
        public const int OfferCount = 3;

        [SerializeField] RouletteGame game;

        public event Action OnChanged;

        public List<PlakId> currentOffers = new List<PlakId>();
        public bool IsOpen { get; private set; }

        System.Random rng = new System.Random();
        static PlakId[] allPlaks;

        void Awake()
        {
            if (game == null) game = GetComponent<RouletteGame>();
            if (allPlaks == null) allPlaks = (PlakId[])Enum.GetValues(typeof(PlakId));
        }

        public bool CanOpen()
        {
            return game != null && game.inventory != null && game.inventory.bossChips > 0;
        }

        /// <summary>Consume one boss chip and roll 3 random plak offers.</summary>
        public bool Open()
        {
            if (!CanOpen() || IsOpen) return false;
            game.inventory.bossChips--;

            currentOffers.Clear();
            var pool = new List<PlakId>(allPlaks);
            for (int i = 0; i < OfferCount && pool.Count > 0; i++)
            {
                int pick = rng.Next(pool.Count);
                currentOffers.Add(pool[pick]);
                pool.RemoveAt(pick);
            }

            IsOpen = true;
            OnChanged?.Invoke();
            return true;
        }

        public void Close()
        {
            IsOpen = false;
            currentOffers.Clear();
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Finalize a plak choice. For plaks that need picks, pass chosenNumbers:
        /// - Color plaks / PlusMinus / Crossed / Plus: the picked numbers.
        /// - Green: [0] = source, [1..3] = replaced positions.
        /// - Gray: pass null (the number is rolled randomly here).
        /// </summary>
        public void Apply(PlakId id, int[] chosenNumbers)
        {
            if (!IsOpen) return;
            if (!currentOffers.Contains(id)) return;

            var plak = new PlakInstance { Id = id };

            switch (id)
            {
                case PlakId.Gray:
                    plak.TargetNumbers = new[] { rng.Next(0, 37) };
                    break;
                default:
                    plak.TargetNumbers = chosenNumbers ?? Array.Empty<int>();
                    break;
            }

            game.inventory.plaks.Add(plak);
            Close();
        }
    }
}
