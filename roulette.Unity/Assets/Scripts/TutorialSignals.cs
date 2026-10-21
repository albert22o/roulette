using System;

public enum TutorialSignal
{
    BetPlaced,
    SpinStarted,
    SpinFinished,
    QuotaCompleted,
    BetFinished,
    LeftTable,
    ShopEntered,
    ThirdQuotaStarted,
    ThirdQuotaRewardsChosen,
    AltarEntered,
    TableEntered,
}

public class TutorialSignals
{
    public event Action<TutorialSignal> Raised;
    public void Raise(TutorialSignal signal)
    {
        UnityEngine.Debug.Log($"[Tutorial] Raise {signal}, subscribers: {Raised != null}");
        Raised?.Invoke(signal);
    }
}
