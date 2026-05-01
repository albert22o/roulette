using System.Collections.Generic;

namespace DeadlyRoulette
{
    public enum BetColor { None, Red, Black }

    public enum BetCategory
    {
        Color,      // x2
        Dozen,      // x3
        Number,     // x5
        Zero        // x10
    }

    public static class RouletteData
    {
        public const int MinNumber = 0;
        public const int MaxNumber = 36;

        public static readonly HashSet<int> Reds = new HashSet<int>
        {
            1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
        };

        public static readonly HashSet<int> Blacks = new HashSet<int>
        {
            2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35
        };

        public static BetColor ColorOf(int n)
        {
            if (n == 0) return BetColor.None;
            return Reds.Contains(n) ? BetColor.Red : BetColor.Black;
        }

        // 1..3 ; 0 -> 0
        public static int DozenOf(int n) => n == 0 ? 0 : ((n - 1) / 12) + 1;

        public static int MultiplierFor(BetCategory c)
        {
            switch (c)
            {
                case BetCategory.Color: return 2;
                case BetCategory.Dozen: return 3;
                case BetCategory.Number:return 5;
                case BetCategory.Zero:  return 10;
            }
            return 0;
        }
    }
}
