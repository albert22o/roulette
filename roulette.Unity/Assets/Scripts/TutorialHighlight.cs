using System;

public enum TutorialTargetId
{
    Quota, Roulette, Ball, FinishBetButton,
    ShopChips, ShopFood, ShopConverter, ShopBallMachine,
    Multipliers,
}

public class TutorialHighlight
{
    public event Action<TutorialTargetId, bool, bool> HighlightRequested;

    public void Set(TutorialTargetId id, bool enabled, bool blink = false)
    {
        HighlightRequested?.Invoke(id, enabled, blink);
    }
}