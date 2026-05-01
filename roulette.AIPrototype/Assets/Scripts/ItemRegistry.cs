using System.Collections.Generic;

namespace DeadlyRoulette
{
    public static class ItemRegistry
    {
        static ItemDefinition Chip(string name, string desc, Rarity r) =>
            new ItemDefinition(name, desc, ItemType.Chip, r);
        static ItemDefinition Card(string name, string desc, Rarity r) =>
            new ItemDefinition(name, desc, ItemType.Card, r);
        static ItemDefinition Food(string name, string desc, Rarity r) =>
            new ItemDefinition(name, desc, ItemType.Food, r);
        static ItemDefinition Ball(string name, string desc, Rarity r) =>
            new ItemDefinition(name, desc, ItemType.Ball, r);

        public static readonly Dictionary<ChipId, ItemDefinition> Chips = new Dictionary<ChipId, ItemDefinition>
        {
            { ChipId.Bordeaux,       Chip("Bordeaux Chip",        "+1 mult on red color wins.",                                        Rarity.Common) },
            { ChipId.Dark,           Chip("Dark Chip",            "+1 mult on black color wins.",                                      Rarity.Common) },
            { ChipId.BlackDozen,     Chip("Black Dozen Chip",     "Random dozen gets +2 mult.",                                        Rarity.Common) },
            { ChipId.Seven,          Chip("Seven Chip",           "4 random numbers get +4 mult.",                                     Rarity.Common) },
            { ChipId.Zero,           Chip("Zero Chip",            "0 gets +10 mult.",                                                  Rarity.Rare)   },
            { ChipId.CrossedTwelve,  Chip("Crossed 12 Chip",      "Random dozen: free reroll on first hit per bet.",                   Rarity.Common) },
            { ChipId.AntiRed,        Chip("Anti-Red Chip",        "Free reroll on first red number per bet.",                          Rarity.Common) },
            { ChipId.AntiBlack,      Chip("Anti-Black Chip",      "Free reroll on first black number per bet.",                        Rarity.Common) },
            { ChipId.Spikes,         Chip("Spikes Chip",          "Replace 3 random numbers with a player-chosen number (keeps color/mods).", Rarity.Rare)   },
            { ChipId.OneThirtySix,   Chip("1-36 Chip",            "Rolling 1, 2, 35 or 36 also counts as rolling 0.",                  Rarity.Epic)   },
            { ChipId.Even,           Chip("Even Chip",            "+1 mult when an even number is rolled.",                            Rarity.Common) },
            { ChipId.Odd,            Chip("Odd Chip",             "+1 mult when an odd number is rolled.",                             Rarity.Common) },
            { ChipId.Prime,          Chip("Prime Chip",           "+2 mult when a prime number is rolled.",                            Rarity.Common) },
            { ChipId.High,           Chip("High Chip",            "+1 mult when rolled number > 18.",                                  Rarity.Rare)   },
            { ChipId.Low,            Chip("Low Chip",             "+1 mult when rolled number < 18.",                                  Rarity.Rare)   },
            { ChipId.Reroll,         Chip("Reroll Chip",          "+2 rerolls per quota.",                                             Rarity.Common) },
            { ChipId.Stack,          Chip("Stack Chip",           "+1 bet per quota.",                                                 Rarity.Rare)   },
            { ChipId.Clover,         Chip("Clover Chip",          "-2 rerolls per quota, +2 mult.",                                    Rarity.Rare)   },
            { ChipId.Coin,           Chip("Coin Chip",            "-1 bet per quota, +2 mult.",                                        Rarity.Rare)   },
            { ChipId.Skull,          Chip("Skull Chip",           "If bets run out without beating quota, quota passes. Destroys after use.", Rarity.Rare)   },
            { ChipId.Percent,        Chip("Percent Chip",         "Reduces quota by 25%.",                                             Rarity.Rare)   },
            { ChipId.GoldBar,        Chip("Gold Bar Chip",        "+mult equal to your gold chip count.",                              Rarity.Rare)   },
            { ChipId.Investment,     Chip("Investment Chip",      "+400 base money per quota.",                                        Rarity.Common) },
            { ChipId.Mining,         Chip("Mining Chip",          "+2 base quota reward.",                                             Rarity.Common) },
            { ChipId.Empty,          Chip("Empty Chip",           "+2 chip slots.",                                                    Rarity.Rare)   },
            { ChipId.Dots,           Chip("Dots Chip",            "+1 mult per other equipped chip.",                                  Rarity.Common) },
            { ChipId.Loss,           Chip("Loss Chip",            "-1 chip slot, but x2 mult.",                                        Rarity.Epic)   },
            { ChipId.BallChip,       Chip("Ball Chip",            "+1 mult per ball owned.",                                           Rarity.Common) },
            { ChipId.EightBall,      Chip("8-Ball Chip",          "+1 ball slot.",                                                     Rarity.Rare)   },
            { ChipId.Checkmark,      Chip("Checkmark Chip",       "+1 ball choice.",                                                   Rarity.Epic)   },
            { ChipId.AntiBall,       Chip("Anti-Ball Chip",       "-1 ball slot, +3 mult.",                                            Rarity.Rare)   },
            { ChipId.D6,             Chip("D6 Chip",              "Balls chosen randomly, +2 mult.",                                   Rarity.Rare)   },
            { ChipId.X,              Chip("X Chip",               "+1 mult.",                                                          Rarity.Common) },
            { ChipId.RedBank,        Chip("Red Bank Chip",        "+0 mult, gains +1 after each red roll.",                            Rarity.Rare)   },
            { ChipId.BlackBank,      Chip("Black Bank Chip",      "+0 mult, gains +1 after each black roll.",                          Rarity.Rare)   },
            { ChipId.VaBank,         Chip("Va-Bank Chip",         "x3 mult if the only bet is on 0.",                                  Rarity.Epic)   },
            { ChipId.Luck4,          Chip("Luck/4 Chip",          "x2 mult if rolled number is divisible by 4.",                       Rarity.Rare)   },
            { ChipId.Glass,          Chip("Glass Chip",           "All non-zero rolls count as one chosen number (0 excluded). Destroys after hit.", Rarity.Rare)   },
            { ChipId.Holey,          Chip("Holey Chip",           "+2 mult per empty chip slot.",                                       Rarity.Common) },
            { ChipId.Champion,       Chip("Champion Chip",        "x1 mult, gains +x0.25 per quota beaten at 2x.",                     Rarity.Rare)   },
            { ChipId.Three,          Chip("Three Chip",           "+2 mult if rolled number divisible by 3.",                          Rarity.Common) },
            { ChipId.MultiBet,       Chip("MultiBet Chip",        "Allows betting on one more number.",                                Rarity.Common) },
            { ChipId.MultiBetPlus,   Chip("MultiBet+ Chip",       "Allows betting on two more numbers.",                               Rarity.Rare)   },
            { ChipId.Six,            Chip("Six Chip",             "6 consecutive random numbers get +3 mult.",                         Rarity.Common) },
            { ChipId.Nine,           Chip("Nine Chip",            "x2 mult on 6, 9, 16, 19, 26, 29, 36.",                              Rarity.Rare)   },
            { ChipId.XRed,           Chip("XRed Chip",            "x1.5 mult if all balls land on red.",                               Rarity.Rare)   },
            { ChipId.XBlack,         Chip("XBlack Chip",          "x1.5 mult if all balls land on black.",                             Rarity.Rare)   },
            { ChipId.Transparent,    Chip("Transparent Chip",     "x1 mult, +x0.5 per empty chip slot.",                               Rarity.Rare)   },
            { ChipId.Space,          Chip("Space Chip",           "+2 number mult, -1 to every other base category mult.",             Rarity.Common) },
            { ChipId.AntiRoll,       Chip("Anti-Roll Chip",       "+1 mult per unused reroll across the run.",                         Rarity.Common) },
            { ChipId.PrimeBank,      Chip("Prime Bank Chip",      "+0 mult, +3 per prime rolled. Resets if a non-prime rolls.",        Rarity.Rare)   },
            { ChipId.GreenBank,      Chip("Green Bank Chip",      "+0 mult, +4 per zero rolled.",                                      Rarity.Rare)   },
            { ChipId.Luck6,          Chip("Luck/6 Chip",          "x3 mult if rolled number is divisible by 6.",                       Rarity.Rare)   },
            { ChipId.Luck3,          Chip("Luck/3 Chip",          "x1.5 mult if rolled number is divisible by 3.",                     Rarity.Common) },
            { ChipId.Budget,         Chip("Budget Chip",          "+50 base money per bet used this quota.",                           Rarity.Common) },
            { ChipId.RerollReward,   Chip("Reroll Reward Chip",   "+1 quota reward per unused reroll.",                                Rarity.Common) },
            { ChipId.LastChance,     Chip("Last Chance Chip",     "x3 mult if 0 rerolls remaining.",                                   Rarity.Rare)   },
            { ChipId.EncoreMore,     Chip("Encore Chip",          "x2 mult on the last bet of the quota.",                             Rarity.Rare)   },
            { ChipId.LuckyNumber,    Chip("Lucky Number Chip",    "x1 mult, +x0.2 per won number bet.",                                Rarity.Rare)   },
        };

        public static readonly Dictionary<CardId, ItemDefinition> Cards = new Dictionary<CardId, ItemDefinition>
        {
            { CardId.SixDiamonds,    Card("6 of Diamonds",  "+1 color mult.",           Rarity.Common) },
            { CardId.SixHearts,      Card("6 of Hearts",    "+1 dozen mult.",           Rarity.Common) },
            { CardId.SixSpades,      Card("6 of Spades",    "+1 dozen mult.",           Rarity.Common) },
            { CardId.SixClubs,       Card("6 of Clubs",     "+1 number mult.",          Rarity.Common) },
            { CardId.GreenSix,       Card("Green 6",        "+1 zero mult.",            Rarity.Common) },
            { CardId.AceDiamonds,    Card("Ace of Diamonds", "+2 color mult.",          Rarity.Rare)   },
            { CardId.AceHearts,      Card("Ace of Hearts",  "+2 dozen mult.",           Rarity.Rare)   },
            { CardId.AceSpades,      Card("Ace of Spades",  "+2 dozen mult.",           Rarity.Rare)   },
            { CardId.AceClubs,       Card("Ace of Clubs",   "+2 number mult.",          Rarity.Rare)   },
            { CardId.GreenAce,       Card("Green Ace",      "+2 zero mult.",            Rarity.Rare)   },
            { CardId.RedJoker,       Card("Red Joker",      "+1 all category mults.",   Rarity.Rare)   },
            { CardId.BlackJoker,     Card("Black Joker",    "+2 all category mults.",   Rarity.Rare)   },
        };

        public static readonly Dictionary<FoodId, ItemDefinition> Foods = new Dictionary<FoodId, ItemDefinition>
        {
            { FoodId.Lemon,      Food("Lemon",      "+1 bet per quota permanently.",                          Rarity.Rare) },
            { FoodId.Lime,       Food("Lime",       "+1 reroll per quota permanently.",                       Rarity.Rare) },
            { FoodId.Apple,      Food("Apple",      "+1 base quota reward permanently.",                      Rarity.Rare) },
            { FoodId.Pear,       Food("Pear",       "+1 unused-bet bonus permanently.",                       Rarity.Rare) },
            { FoodId.Cherry,     Food("Cherry",     "+200 base money permanently.",                           Rarity.Rare) },
            { FoodId.Banana,     Food("Banana",     "First time balance hits 0 per quota, gain 200 money.",   Rarity.Rare) },
            { FoodId.Orange,     Food("Orange",     "+1 chip slot permanently.",                              Rarity.Rare) },
            { FoodId.Strawberry, Food("Strawberry", "+1 ball slot permanently.",                              Rarity.Rare) },
            { FoodId.Grape,      Food("Grape",      "50% stake refund on losing bet.",                        Rarity.Rare) },
            { FoodId.Chocolate,  Food("Chocolate",  "Reroll cost reduced by 2.",                              Rarity.Rare) },
        };

        public static readonly Dictionary<BallId, ItemDefinition> Balls = new Dictionary<BallId, ItemDefinition>
        {
            { BallId.Normal,       Ball("Normal Ball",       "A standard ball. Nothing special.",                          Rarity.Common) },
            { BallId.Glass,        Ball("Glass Ball",        "x2 mult when chosen. 50% chance to destroy after use.",      Rarity.Rare)   },
            { BallId.Bouncy,       Ball("Bouncy Ball",       "+1 to +5 mult when chosen.",                                 Rarity.Common) },
            { BallId.Stone,        Ball("Stone Ball",        "Always scores even if not chosen.",                           Rarity.Rare)   },
            { BallId.Red,          Ball("Red Ball",          "+5 mult if chosen and landed on red.",                        Rarity.Common) },
            { BallId.Black,        Ball("Black Ball",        "+5 mult if chosen and landed on black.",                      Rarity.Common) },
            { BallId.Green,        Ball("Green Ball",        "x2 mult if chosen and landed on 0.",                          Rarity.Rare)   },
            { BallId.Explosive,    Ball("Explosive Ball",    "+1 to +10 mult when chosen. 15% chance to destroy.",          Rarity.Rare)   },
            { BallId.RedMagnet,    Ball("Red Magnet Ball",   "Rerolls until it lands on red. Destroys after 5 bets.",      Rarity.Epic)   },
            { BallId.BlackMagnet,  Ball("Black Magnet Ball", "Rerolls until it lands on black. Destroys after 5 bets.",    Rarity.Epic)   },
        };
    }
}
