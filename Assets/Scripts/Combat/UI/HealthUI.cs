using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _frontSlider;
    [SerializeField] private Slider _backSlider;

    [SerializeField] private Image _frontFill;
    [SerializeField] private Image _background;

    [Header("Health Colors")]
    [SerializeField] private Gradient _healthGradient;

    [Header("Animation")]
    [SerializeField] private float _lerpDuration = 0.5f;
    [SerializeField] private float _damageDelay = 0.15f;

    [Header("Damage Flash")]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashDuration = 0.08f;

    [Header("Shake")]
    [SerializeField] private float _shakeDuration = 0.18f;
    [SerializeField] private float _shakeStrength = 8f;
    [SerializeField] private int _shakeVibrato = 20;
    [SerializeField] private float _shakeRandomness = 90f;

    [SerializeField] private RectTransform _rect;
    private Tween _shakeTween;

    private void Awake()
    {
        _frontSlider.value = 1f;
        _backSlider.value = 1f;

        UpdateColor(1f);
    }
    private void UpdateColor(float percent)
    {
        _frontFill.color = _healthGradient.Evaluate(percent);
    }

    /// <summary>
    /// Sets the health value
    /// healthPercent should be between 0 and 1.
    /// </summary>
    public void SetHealth(float healthPercent)
    {
        healthPercent = Mathf.Clamp01(healthPercent);

        bool healthChange = healthPercent != _frontSlider.value;
        bool damage = healthPercent < _frontSlider.value;

        if (healthChange)
        {
            _frontSlider.value = healthPercent;

            _backSlider.DOValue(healthPercent, _lerpDuration)
                .SetEase(Ease.OutCubic)
                .SetDelay(_damageDelay)
                .SetLink(gameObject);

            _frontFill.DOColor(_flashColor, _flashDuration)
                .OnComplete(() =>
                {
                    _frontFill.DOColor(
                        _healthGradient.Evaluate(healthPercent),
                        0.12f)
                        .SetLink(gameObject);
                }).SetLink(gameObject);

            if (damage)
            {
                ApplyShake();
            }
        }
        else
        {
            _frontSlider.DOValue(healthPercent, 0.25f).SetLink(gameObject);
            _backSlider.DOValue(healthPercent, 0.25f).SetLink(gameObject);
        }
    }

    private void ApplyShake()
    {
        _shakeTween?.Kill();

        _rect.anchoredPosition = Vector2.zero;

        _shakeTween = _rect
            .DOShakeAnchorPos(
                _shakeDuration,
                _shakeStrength,
                _shakeVibrato,
                _shakeRandomness,
                false,
                true)
            .SetLink(gameObject)
            .OnComplete(() => _rect.anchoredPosition = Vector2.zero);
    }
}
