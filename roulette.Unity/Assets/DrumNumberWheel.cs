using DG.Tweening;
using UnityEngine;

public class DrumNumberWheel : MonoBehaviour
{
    [SerializeField][Range(0, 9)] private int currentDigit = 0;
    [SerializeField] private float duration = 0.3f;

    private Tween rotationTween;
    private const float DegreesPerDigit = 36f;

    public int CurrentDigit
    {
        get => currentDigit;
        set
        {
            currentDigit = Mathf.Clamp(value, 0, 9);
            UpdateRotation();
        }
    }

    private void OnValidate()
    {
        UpdateRotation();
    }

    private void Start()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float targetAngle = currentDigit * DegreesPerDigit;

        rotationTween?.Kill();
        rotationTween = transform.DOLocalRotate(new Vector3(0f, 0f, targetAngle), duration, RotateMode.Fast)
            .SetEase(Ease.InOutElastic, 0.2f);
    }

    private void OnDestroy()
    {
        rotationTween?.Kill();
    }
}
