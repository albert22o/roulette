using UnityEngine;

public class TimeScaleManager
{
    private bool isPaused = false;

    public float TargetTimeScale { get; private set; } = 1f;

    private TimeScaleManager()
    {
        TargetTimeScale = Time.timeScale;
    }

    public void SetTimeScale(float newScale)
    {
        TargetTimeScale = Mathf.Max(0f, newScale);

        if (!isPaused)
        {
            Time.timeScale = TargetTimeScale;
        }
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;

        if (isPaused)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = TargetTimeScale; 
        }
    }

    public void ResetTimeScale()
    {
        isPaused = false;
        TargetTimeScale = 1f;
        Time.timeScale = 1f;
    }
}
