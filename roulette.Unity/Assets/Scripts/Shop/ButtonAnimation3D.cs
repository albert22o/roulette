using DG.Tweening;
using UnityEngine;

public class ButtonAnimation3D : MonoBehaviour
{
    [SerializeField] private Vector3 pushOffset = new Vector3(0, -0.1f, 0); 
    [SerializeField] private float duration = 0.1f;

    private Vector3 _startPosition;
    private Tween _pressTween;

    private void Awake()
    {
        _startPosition = transform.localPosition;
    }

    public void Press()
    {
        _pressTween?.Kill();

        _pressTween = transform.DOLocalMove(_startPosition + pushOffset, duration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }
}
