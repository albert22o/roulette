using DG.Tweening;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DrumNumberWheel : MonoBehaviour
{
    [SerializeField][Range(0, 9)] private int currentDigit = 0;
    [SerializeField] private float duration = 0.3f;

    private Tween rotationTween;
    private const float DegreesPerDigit = 36f;

    public int CurrentDigit => currentDigit;

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
            .SetEase(Ease.InOutElastic, 0.2f);
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
