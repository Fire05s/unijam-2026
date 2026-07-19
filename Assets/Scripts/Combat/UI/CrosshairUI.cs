using DG.Tweening;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _crosshair;

    [Header("Rotation")]
    [SerializeField] private bool _rotate = true;
    [SerializeField] private float _rotationDuration = 2f;
    [SerializeField] private bool _clockwise = true;

    [Header("Pulse")]
    [SerializeField] private bool _pulse = true;
    [SerializeField] private float _pulseScale = 1.1f;
    [SerializeField] private float _pulseDuration = 0.5f;

    private Tween _rotationTween;
    private Tween _pulseTween;

    private void OnEnable()
    {
        if (_rotate)
        {
            float endRotation = _clockwise ? -360f : 360f;

            _rotationTween = _crosshair
                .DORotate(
                    new Vector3(0, 0, endRotation),
                    _rotationDuration,
                    RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject);
        }

        if (_pulse)
        {
            _pulseTween = _crosshair
                .DOScale(_pulseScale, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }
    }

    private void OnDisable()
    {
        _rotationTween?.Kill();
        _pulseTween?.Kill();

        _crosshair.localRotation = Quaternion.identity;
        _crosshair.localScale = Vector3.one;
    }
}
