using System;
using System.Collections.Generic;

namespace DeadlyRoulette
{
    /// <summary>
    /// Permanent field modifiers bought with boss chips.
    /// Unlike chips, plaks are not destroyed when triggered and are not sold.
    /// </summary>
    public enum PlakId
    {
        Black,          // recolor 5 chosen red numbers -> black
        Red,            // recolor 5 chosen black numbers -> red
        White,          // recolor 2 chosen numbers -> white
        Blue,           // recolor 4 chosen numbers -> blue
        Gray,           // recolor 1 random number -> gray, permanent +3 mult
        Purple,         // recolor 2 chosen numbers -> purple (+5 mult on hit)
        Green,          // pick 1 number, 3 others are replaced with it
        Orange,         // recolor 2 chosen numbers -> orange
        Yellow,         // recolor 2 chosen numbers -> yellow
        PlusMinus,      // chosen number gets ±2 tag
        Crossed,        // remove 3 chosen numbers from field + wheel
        Plus            // add 2 copies of chosen number to field + wheel
    }

    [Serializable]
    public class PlakInstance
    {
        public PlakId Id;

        /// <summary>
        /// Meaning depends on plak type:
        ///  - Color plaks (Black/Red/White/Blue/Gray/Purple/Orange/Yellow): list of recolored numbers.
        ///  - Green: [0] = chosen number, [1..] = replaced positions.
        ///  - PlusMinus: [0] = tagged number.
        ///  - Crossed: list of removed numbers.
        ///  - Plus: [0] = number whose copies are added to the pool (2 copies).
        /// </summary>
        public int[] TargetNumbers;

        /// <summary>
        /// For the White plak: numbers that have already been triggered and
        /// reverted to their default color. Checked by GetNumberColor.
        /// </summary>
        public List<int> TriggeredNumbers;
    }

    public struct PlakDefinition
    {
        public string Name;
        public string Description;
        /// <summary>Numbers the player must pick when taking the plak. 0 = fully random.</summary>
        public int NumbersToPick;
        /// <summary>Restrict the picker to numbers of this color (None = any number).</summary>
        public BetColor PickColorRestriction;
        /// <summary>True if this plak also picks a single "source" number before the count picker (Green).</summary>
        public bool PicksSourceFirst;

        public PlakDefinition(string name, string desc, int picks, BetColor colorRestrict = BetColor.None, bool sourceFirst = false)
        {
            Name = name;
            Description = desc;
            NumbersToPick = picks;
            PickColorRestriction = colorRestrict;
            PicksSourceFirst = sourceFirst;
        }
    }

    public static class PlakRegistry
    {
        public static readonly Dictionary<PlakId, PlakDefinition> Plaks = new Dictionary<PlakId, PlakDefinition>
        {
            { PlakId.Black,     new PlakDefinition("Black Plak",      "Recolor 5 red numbers to black.",                5, BetColor.Red) },
            { PlakId.Red,       new PlakDefinition("Red Plak",        "Recolor 5 black numbers to red.",                5, BetColor.Black) },
            { PlakId.White,     new PlakDefinition("White Plak",      "2 numbers become white: hitting them wins the quota instantly (once each).", 2) },
            { PlakId.Blue,      new PlakDefinition("Blue Plak",       "4 numbers become blue: first hit per bet gives a free reroll.", 4) },
            { PlakId.Gray,      new PlakDefinition("Gray Plak",       "A random number becomes gray (instant loss) and you gain +3 permanent mult.", 0) },
            { PlakId.Purple,    new PlakDefinition("Purple Plak",     "2 numbers become purple: +5 mult when they hit.", 2) },
            { PlakId.Green,     new PlakDefinition("Green Plak",      "Pick a number, then 3 other numbers that are replaced by it.", 3, BetColor.None, true) },
            { PlakId.Orange,    new PlakDefinition("Orange Plak",     "2 numbers become orange: hitting them adds +1 reroll.", 2) },
            { PlakId.Yellow,    new PlakDefinition("Yellow Plak",     "2 numbers become yellow: hitting them gives +2 gold chips.", 2) },
            { PlakId.PlusMinus, new PlakDefinition("± Plak",           "A chosen number also counts as hit when the roll lands within ±2.", 1) },
            { PlakId.Crossed,   new PlakDefinition("Crossed Plak",    "Remove 3 chosen numbers from the board and the wheel.",  3) },
            { PlakId.Plus,      new PlakDefinition("+ Plak",           "Add 2 extra copies of a chosen number to the board and wheel.", 1) },
        };
    }
}
