using System;
using System.Diagnostics;

public enum BetHighlightLevel
{
    None = 0,
    Color = 1,
    Dozen = 2,
    Number = 3
}

public class FieldHighlightState
{
    private bool isNumberBet;
    private bool isDozenBet;
    private bool isColorBet;
    private bool isRimHighlighted;
    private BetHighlightLevel currentLevel;

    public event Action<BetHighlightLevel> BetHighlightLevelChanged;
    public event Action<bool> RimHighlightChanged;

    public bool IsNumberBetHighlighted
    {
        get => isNumberBet;
        set { isNumberBet = value; RecalculateLevel(); }
    }

    public bool IsDozenBetHighlighted
    {
        get => isDozenBet;
        set { isDozenBet = value; RecalculateLevel(); }
    }

    public bool IsColorBetHighlighted
    {
        get => isColorBet;
        set { isColorBet = value; RecalculateLevel(); }
    }

    public BetHighlightLevel BetHighlightLevel => currentLevel;

    public bool IsRimHighlighted
    {
        get => isRimHighlighted;
        set
        {
            if (isRimHighlighted != value)
            {
                isRimHighlighted = value;
                RimHighlightChanged?.Invoke(isRimHighlighted);
            }
        }
    }

    private void RecalculateLevel()
    {
        BetHighlightLevel newLevel =
            isNumberBet ? BetHighlightLevel.Number :
            isDozenBet ? BetHighlightLevel.Dozen :
            isColorBet ? BetHighlightLevel.Color :
            BetHighlightLevel.None;

        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            BetHighlightLevelChanged?.Invoke(currentLevel);
        }
    }
}