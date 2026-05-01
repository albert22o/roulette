namespace DeadlyRoulette
{
    public enum ItemType { Chip, Card, Food, Ball }

    public enum Rarity { Common, Rare, Epic }

    /// <summary>
    /// Colors that numbers on the board can be recolored to by chip effects.
    /// Default means the number uses its standard red/black/green(0) color.
    /// </summary>
    public enum NumberColor
    {
        Default,
        Red,
        Black,
        Gray,       // instant loss on roll
        Purple,     // +5 number mult on roll
        Blue,       // free reroll on first hit per blue number
        White,      // balance = quota, return stake, then number reverts to default
        Yellow,     // +2 gold chips on hit
        Orange      // +1 reroll on hit
    }

    public enum ChipId
    {
        Bordeaux,           // Red color roll +1 mult
        Dark,               // Black color roll +1 mult
        BlackDozen,         // Random dozen +2 mult
        Seven,              // 4 random numbers +4 mult
        Zero,               // 0 gets +10 mult
        CrossedTwelve,      // Random dozen free reroll on first hit per bet
        AntiRed,            // Free reroll on first red per bet
        AntiBlack,          // Free reroll on first black per bet
        Spikes,             // Replace 3 random numbers with chosen number
        OneThirtySix,       // 1, 2, 35 and 36 also count as 0
        Even,               // Even number +1 mult
        Odd,                // Odd number +1 mult
        Prime,              // Prime number +2 mult
        High,               // >18 gives +1 mult
        Low,                // <18 gives +1 mult
        Reroll,             // +2 rerolls per quota
        Stack,              // +1 bet per quota
        Clover,             // -2 rerolls, +2 mult passive
        Coin,               // -1 bet, +2 mult passive
        Skull,              // Saves from game over once, destroys
        Percent,            // -25% quota
        GoldBar,            // +mult equal to gold chips count
        Investment,         // +400 base money per quota
        Mining,             // +2 base quota reward
        Empty,              // +2 chip slots
        Dots,               // +1 mult per other chip
        Loss,               // -1 chip slot, x2 mult
        BallChip,           // +1 mult per ball owned
        EightBall,          // +1 ball slot
        Checkmark,          // +1 ball choice
        AntiBall,           // -1 ball slot, +3 mult
        D6,                 // Balls chosen randomly, +2 mult passive
        X,                  // +1 mult
        RedBank,            // +0 base, +1 after each red roll
        BlackBank,          // +0 base, +1 after each black roll
        VaBank,             // x3 mult if sole bet is on 0
        Luck4,              // x2 mult if rolled divisible by 4
        Glass,              // Non-zero rolls = one chosen number, destroys after hit
        Holey,              // +2 mult per empty chip slot
        Champion,           // x1 mult + x0.25 per quota beaten 2x
        Three,              // +2 mult if rolled divisible by 3
        MultiBet,           // +1 number betting slot
        MultiBetPlus,       // +2 number betting slots
        Six,                // 6 consecutive random numbers get +3 mult
        Nine,               // x2 mult on 6,9,16,19,26,29,36
        XRed,               // x1.5 mult if all balls land on red
        XBlack,             // x1.5 mult if all balls land on black
        Transparent,        // x1 + x0.5 per empty chip slot
        Space,              // +2 number mult, -1 to all other base category mults
        AntiRoll,           // +1 mult per unused reroll across the run (accumulator)
        PrimeBank,          // +0 mult, +3 per prime rolled, resets on non-prime
        GreenBank,          // +0 mult, +4 per zero rolled
        Luck6,              // x3 mult if rolled divisible by 6
        Luck3,              // x1.5 mult if rolled divisible by 3
        Budget,             // +50 base money per bet used
        RerollReward,       // +1 quota reward per unused reroll
        LastChance,         // x3 mult if 0 rerolls remaining
        EncoreMore,         // x2 mult on last bet of quota
        LuckyNumber         // x1 + x0.2 per won number bet (accumulator)
    }

    public enum CardId
    {
        SixDiamonds,        // +1 color mult
        SixHearts,          // +1 dozen mult
        SixSpades,          // +1 dozen mult
        SixClubs,           // +1 number mult
        GreenSix,           // +1 zero mult
        AceDiamonds,        // +2 color mult
        AceHearts,          // +2 dozen mult
        AceSpades,          // +2 dozen mult
        AceClubs,           // +2 number mult
        GreenAce,           // +2 zero mult
        RedJoker,           // +1 all mults
        BlackJoker          // +2 all mults
    }

    public enum FoodId
    {
        Lemon,              // +1 bet per quota permanently
        Lime,               // +1 reroll per quota permanently
        Apple,              // +1 base reward
        Pear,               // +1 unused bet bonus
        Cherry,             // +200 base money permanently
        Banana,             // 200 money when balance hits 0 (once per quota)
        Orange,             // +1 chip slot
        Strawberry,         // +1 ball slot
        Grape,              // 50% stake refund on loss
        Chocolate           // Reroll cost -2
    }

    public enum BallId
    {
        Normal,             // Nothing special
        Glass,              // x2 mult when chosen, 50% destroy chance
        Bouncy,             // +1 to +5 mult when chosen
        Stone,              // Always scores even if not chosen
        Red,                // +5 mult if landed on red
        Black,              // +5 mult if landed on black
        Green,              // x2 mult if landed on 0
        Explosive,          // +1 to +10 mult, 15% destroy chance
        RedMagnet,          // Rerolls until red, destroys after 5 bets
        BlackMagnet         // Rerolls until black, destroys after 5 bets
    }

    public struct ItemDefinition
    {
        public string Name;
        public string Description;
        public ItemType Type;
        public Rarity Rarity;

        public ItemDefinition(string name, string desc, ItemType type, Rarity rarity)
        {
            Name = name;
            Description = desc;
            Type = type;
            Rarity = rarity;
        }
    }
}
