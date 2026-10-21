using System;

public class TutorialLog
{
    public const float FadeInDuration = 0.2f;
    public const float FadeOutDuration = 0.4f;

    public event Action<string, float> LogRequested;
    public event Action<float> HideRequested;

    /// <param name="duration">Сколько держать текст. 0 и меньше означает "до HideLog".</param>
    public void ShowLog(string msg, float duration)
    {
        LogRequested?.Invoke(msg, duration);
    }

    public void HideLog(float fadeDuration = FadeOutDuration)
    {
        HideRequested?.Invoke(fadeDuration);
    }
}
