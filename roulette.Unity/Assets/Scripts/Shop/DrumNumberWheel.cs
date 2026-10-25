using DG.Tweening;
using System;
using UnityEngine;

public class DrumNumberWheel : MonoBehaviour
{
    [SerializeField][Range(0, 9)] private int currentDigit = 0;
    [SerializeField] private float duration = 0.3f;

    private Tween rotationTween;
    private const float DegreesPerDigit = 36f;

    public int CurrentDigit => currentDigit;


    private int lastTriggeredDigit;

    public event Action WheelStepPassed;
    public event Action GoalNumberReached;

    private void Start()
    {
        UpdateRotationImmediately();
    }

    public void SetDigit(int digit, bool setImmediately)
    {
        currentDigit = Mathf.Clamp(digit, 0, 9);
        if (setImmediately)
        {
            UpdateRotationImmediately();
        }
        else
        {
            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        float targetAngle = currentDigit * DegreesPerDigit;

        rotationTween?.Kill();

        rotationTween = transform.DOLocalRotate(new Vector3(0f, 0f, targetAngle), duration, RotateMode.Fast)
            .SetEase(Ease.InOutElastic, 0.2f)
            .OnUpdate(CheckForClick)
            .OnComplete(() => GoalNumberReached?.Invoke());
    }

    private void CheckForClick()
    {
        float currentAngle = transform.localEulerAngles.z;

        int closestDigit = Mathf.RoundToInt(currentAngle / DegreesPerDigit) % 10;

        if (closestDigit != lastTriggeredDigit)
        {
            lastTriggeredDigit = closestDigit;
            WheelStepPassed?.Invoke(); 
        }
    }

    private void UpdateRotationImmediately()
    {
        float targetAngle = currentDigit * DegreesPerDigit;
        transform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private void OnDestroy()
    {
        rotationTween?.Kill();
    }
}
